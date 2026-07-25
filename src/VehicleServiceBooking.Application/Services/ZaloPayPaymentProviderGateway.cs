using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VehicleServiceBooking.Application.Configuration;
using VehicleServiceBooking.Application.Interfaces.Services;
using VehicleServiceBooking.Domain.Enums;

namespace VehicleServiceBooking.Application.Services;

/// <summary>
/// Real ZaloPay gateway integration for create-intent checkout links.
/// </summary>
public sealed class ZaloPayPaymentProviderGateway : IPaymentProviderGateway
{
    private readonly HttpClient _httpClient;
    private readonly PaymentProviderGatewayOptions _options;
    private readonly ILogger<ZaloPayPaymentProviderGateway> _logger;

    public ZaloPayPaymentProviderGateway(
        HttpClient httpClient,
        IOptions<PaymentProviderGatewayOptions> options,
        ILogger<ZaloPayPaymentProviderGateway> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<PaymentProviderIntentResult> CreateIntentAsync(
        PaymentProviderIntentRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.PaymentProviderType != PaymentProviderType.ZaloPay)
        {
            throw new InvalidOperationException(
                $"ZaloPay gateway cannot process provider '{request.PaymentProviderType}'.");
        }

        if (request.PaymentMethodType != PaymentMethodType.AtmCardDomestic)
        {
            throw new InvalidOperationException(
                $"ZaloPay real integration currently supports only '{PaymentMethodType.AtmCardDomestic}'.");
        }

        var zaloOptions = _options.ZaloPay;
        if (!zaloOptions.Enabled)
        {
            throw new InvalidOperationException("ZaloPay gateway is disabled by configuration.");
        }

        var utcNow = DateTimeOffset.UtcNow;
        var appTime = utcNow.ToUnixTimeMilliseconds();
        var amount = ToProviderAmount(request.Amount);
        var appTransId = BuildAppTransId(request.IntentCode, utcNow);
        var appUser = BuildAppUser(zaloOptions.AppUserPrefix, request.OrderId);

        var embedData = JsonSerializer.Serialize(new Dictionary<string, string>
        {
            ["redirecturl"] = zaloOptions.RedirectUrl,
            ["intentCode"] = request.IntentCode,
            ["orderId"] = request.OrderId.ToString()
        });

        const string item = "[]";
        var dataToSign = $"{zaloOptions.AppId}|{appTransId}|{appUser}|{amount}|{appTime}|{embedData}|{item}";
        var mac = ComputeMac(zaloOptions.Key1, dataToSign);

        var formData = new Dictionary<string, string>
        {
            ["app_id"] = zaloOptions.AppId.ToString(CultureInfo.InvariantCulture),
            ["app_user"] = appUser,
            ["app_time"] = appTime.ToString(CultureInfo.InvariantCulture),
            ["amount"] = amount.ToString(CultureInfo.InvariantCulture),
            ["app_trans_id"] = appTransId,
            ["embed_data"] = embedData,
            ["item"] = item,
            ["description"] = $"Vehicle service payment for order {request.OrderId}",
            ["bank_code"] = "ATM",
            ["callback_url"] = zaloOptions.CallbackUrl,
            ["mac"] = mac
        };

        using var content = new FormUrlEncodedContent(formData);
        using var response = await _httpClient
            .PostAsync(zaloOptions.CreateOrderPath, content, cancellationToken)
            .ConfigureAwait(false);

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"ZaloPay create intent failed with status {(int)response.StatusCode}: {responseBody}");
        }

        var providerResponse = JsonSerializer.Deserialize<ZaloPayCreateOrderResponse>(responseBody,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (providerResponse == null)
        {
            throw new InvalidOperationException("ZaloPay create intent returned an empty response.");
        }

        if (providerResponse.ReturnCode != 1 || string.IsNullOrWhiteSpace(providerResponse.OrderUrl))
        {
            throw new InvalidOperationException(
                $"ZaloPay create intent returned code '{providerResponse.ReturnCode}': {providerResponse.ReturnMessage}");
        }

        _logger.LogInformation(
            "Created ZaloPay intent: intentCode={IntentCode}, appTransId={AppTransId}, returnCode={ReturnCode}",
            request.IntentCode,
            appTransId,
            providerResponse.ReturnCode);

        return new PaymentProviderIntentResult
        {
            CheckoutUrl = providerResponse.OrderUrl,
            ExpiresAtUtc = utcNow.UtcDateTime.AddMinutes(zaloOptions.IntentTtlMinutes)
        };
    }

    private static long ToProviderAmount(decimal amount)
    {
        if (amount <= 0)
        {
            throw new InvalidOperationException("Payment amount must be greater than zero.");
        }

        // VND flows use whole-number amounts at provider boundary.
        return decimal.ToInt64(decimal.Round(amount, 0, MidpointRounding.AwayFromZero));
    }

    private static string BuildAppTransId(string intentCode, DateTimeOffset utcNow)
    {
        var sanitizedIntent = intentCode.Replace("-", string.Empty, StringComparison.Ordinal);
        var suffix = sanitizedIntent.Length > 24 ? sanitizedIntent[..24] : sanitizedIntent;
        return $"{utcNow:yyMMdd}_{suffix}";
    }

    private static string BuildAppUser(string prefix, Guid orderId)
    {
        var effectivePrefix = string.IsNullOrWhiteSpace(prefix) ? "vsb" : prefix.Trim();
        var appUser = $"{effectivePrefix}_{orderId:N}";
        return appUser.Length <= 64 ? appUser : appUser[..64];
    }

    private static string ComputeMac(string secret, string data)
    {
        var keyBytes = Encoding.UTF8.GetBytes(secret);
        var dataBytes = Encoding.UTF8.GetBytes(data);

        using var hmac = new HMACSHA256(keyBytes);
        var hash = hmac.ComputeHash(dataBytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private sealed class ZaloPayCreateOrderResponse
    {
        [JsonPropertyName("return_code")]
        public int ReturnCode { get; set; }

        [JsonPropertyName("return_message")]
        public string ReturnMessage { get; set; } = string.Empty;

        [JsonPropertyName("order_url")]
        public string? OrderUrl { get; set; }
    }
}
