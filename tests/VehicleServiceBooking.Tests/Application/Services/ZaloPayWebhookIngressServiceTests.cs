using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using VehicleServiceBooking.Application.Configuration;
using VehicleServiceBooking.Application.Exceptions;
using VehicleServiceBooking.Application.Helpers;
using VehicleServiceBooking.Application.Services;
using VehicleServiceBooking.Domain.Enums;

namespace VehicleServiceBooking.Tests.Application.Services;

public class ZaloPayWebhookIngressServiceTests
{
    [Fact]
    public async Task NormalizeAsync_WithInvalidCallbackMac_ShouldThrowSignatureValidationException()
    {
        var service = CreateService(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"return_code\":1,\"is_processing\":false,\"zp_trans_id\":123456}", Encoding.UTF8, "application/json")
        });

        using var payload = JsonDocument.Parse("""
        {
          "data": "{\"app_trans_id\":\"250723_test123\",\"embed_data\":\"{\\\"intentCode\\\":\\\"PI-123\\\"}\",\"zp_trans_id\":123456,\"server_time\":1721731200000}",
          "mac": "BADMAC",
          "type": 1
        }
        """);

        var act = async () => await service.NormalizeAsync(payload.RootElement, CancellationToken.None);

        await act.Should().ThrowAsync<PaymentWebhookSignatureValidationException>();
    }

    [Fact]
    public async Task NormalizeAsync_WithValidCallbackAndSuccessfulQuery_ShouldReturnNormalizedRequest()
    {
        HttpRequestMessage? capturedRequest = null;
        var service = CreateService(request =>
        {
            capturedRequest = request;
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    "{\"return_code\":1,\"return_message\":\"success\",\"is_processing\":false,\"zp_trans_id\":987654321}",
                    Encoding.UTF8,
                    "application/json")
            };
        });

        var callbackData = "{\"app_trans_id\":\"250723_test123\",\"embed_data\":\"{\\\"redirecturl\\\":\\\"https://localhost/return\\\",\\\"intentCode\\\":\\\"PI-1234567890\\\",\\\"orderId\\\":\\\"9f4ef7cb-a7ad-4d0d-b75d-21c50c1c83cc\\\"}\",\"zp_trans_id\":987654321,\"server_time\":1721731200000}";
        var callbackMac = ComputeMac("test-key2", callbackData);

        using var payload = JsonDocument.Parse($$"""
        {
          "data": {{JsonSerializer.Serialize(callbackData)}},
          "mac": "{{callbackMac}}",
          "type": 1
        }
        """);

        var response = await service.NormalizeAsync(payload.RootElement, CancellationToken.None);

        response.EventId.Should().Be("zalopay-987654321");
        response.IntentCode.Should().Be("PI-1234567890");
        response.TransactionStatus.Should().Be(PaymentTransactionStatus.Completed.ToString());
        response.ProviderTransactionId.Should().Be("987654321");
        response.SignatureHash.Should().NotBeNullOrWhiteSpace();
        response.Payload.Should().Contain("\"data\"");
        response.OccurredAtUtc.Should().Be(DateTimeOffset.FromUnixTimeMilliseconds(1721731200000).UtcDateTime);

        capturedRequest.Should().NotBeNull();
        capturedRequest!.RequestUri!.AbsolutePath.Should().Be("/v2/query");

        var body = await capturedRequest.Content!.ReadAsStringAsync();
        body.Should().Contain("app_id=2553");
        body.Should().Contain("app_trans_id=250723_test123");
    }

    private static ZaloPayWebhookIngressService CreateService(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
    {
        var handler = new StubMessageHandler(responseFactory);
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://sb-openapi.zalopay.vn")
        };

        var gatewayOptions = Options.Create(new PaymentProviderGatewayOptions
        {
            ZaloPay = new ZaloPayGatewayOptions
            {
                Enabled = true,
                BaseUrl = "https://sb-openapi.zalopay.vn",
                CreateOrderPath = "/v2/create",
                QueryOrderPath = "/v2/query",
                AppId = 2553,
                Key1 = "test-key1",
                Key2 = "test-key2",
                CallbackUrl = "https://localhost:5158/api/v1/payments/webhooks/zalopay",
                RedirectUrl = "https://localhost:5158/api/v1/payments/return",
                RequestTimeoutSeconds = 10
            }
        });

        var webhookOptions = Options.Create(new PaymentWebhookSecurityOptions
        {
            Enabled = true,
            RequireSignature = true,
            SharedSecret = "internal-shared-secret"
        });

        return new ZaloPayWebhookIngressService(
            httpClient,
            gatewayOptions,
            webhookOptions,
            Mock.Of<ILogger<ZaloPayWebhookIngressService>>());
    }

    private static string ComputeMac(string secret, string data)
    {
        using var hmac = new System.Security.Cryptography.HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private sealed class StubMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _responseFactory;

        public StubMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            _responseFactory = responseFactory;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_responseFactory(request));
        }
    }
}