using System.Net;
using System.Net.Http;
using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using VehicleServiceBooking.Application.Configuration;
using VehicleServiceBooking.Application.Interfaces.Services;
using VehicleServiceBooking.Application.Services;
using VehicleServiceBooking.Domain.Enums;

namespace VehicleServiceBooking.Tests.Application.Services;

public class ZaloPayPaymentProviderGatewayTests
{
    [Fact]
    public async Task CreateIntentAsync_WhenMethodNotSupported_ShouldThrowInvalidOperationException()
    {
        var gateway = CreateGateway(_ =>
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{}", Encoding.UTF8, "application/json")
            });

        var request = BuildRequest(PaymentMethodType.CreditCard);

        var act = async () => await gateway.CreateIntentAsync(request, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*supports only*AtmCardDomestic*");
    }

    [Fact]
    public async Task CreateIntentAsync_WhenProviderReturnsSuccess_ShouldReturnCheckoutUrlAndExpiry()
    {
        var gateway = CreateGateway(_ =>
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    "{\"return_code\":1,\"return_message\":\"OK\",\"order_url\":\"https://sbgateway.zalopay.vn/checkout/abc\"}",
                    Encoding.UTF8,
                    "application/json")
            });

        var request = BuildRequest(PaymentMethodType.AtmCardDomestic);

        var response = await gateway.CreateIntentAsync(request, CancellationToken.None);

        response.CheckoutUrl.Should().Be("https://sbgateway.zalopay.vn/checkout/abc");
        response.ExpiresAtUtc.Should().BeAfter(DateTime.UtcNow.AddMinutes(10));
    }

    private static ZaloPayPaymentProviderGateway CreateGateway(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
    {
        var handler = new StubMessageHandler(responseFactory);
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://sb-openapi.zalopay.vn")
        };

        var options = Options.Create(new PaymentProviderGatewayOptions
        {
            UseDevelopmentGatewayFallback = true,
            ZaloPay = new ZaloPayGatewayOptions
            {
                Enabled = true,
                BaseUrl = "https://sb-openapi.zalopay.vn",
                CreateOrderPath = "/v2/create",
                AppId = 2553,
                Key1 = "test-key",
                AppUserPrefix = "vsb",
                CallbackUrl = "https://localhost:5158/api/v1/payments/webhooks/zalopay",
                RedirectUrl = "https://localhost:5158/api/v1/payments/return",
                IntentTtlMinutes = 15,
                RequestTimeoutSeconds = 10
            }
        });

        return new ZaloPayPaymentProviderGateway(httpClient, options, Mock.Of<ILogger<ZaloPayPaymentProviderGateway>>());
    }

    private static PaymentProviderIntentRequest BuildRequest(PaymentMethodType methodType)
    {
        return new PaymentProviderIntentRequest
        {
            OrderId = Guid.NewGuid(),
            IntentCode = "PI-20260721112233-abcdefabcdef",
            Amount = 500000m,
            CurrencyId = Guid.NewGuid(),
            PaymentProviderId = Guid.Parse("00000000-0000-0000-0003-000000000001"),
            PaymentMethodId = Guid.Parse("00000000-0000-0000-0008-000000000001"),
            PaymentProviderType = PaymentProviderType.ZaloPay,
            PaymentMethodType = methodType
        };
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
