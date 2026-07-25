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
using VehicleServiceBooking.Application.DTOs;
using VehicleServiceBooking.Application.Exceptions;
using VehicleServiceBooking.Application.Helpers;
using VehicleServiceBooking.Application.Interfaces.Services;
using VehicleServiceBooking.Domain.Enums;

namespace VehicleServiceBooking.Application.Services;

/// <summary>
/// Validates native ZaloPay callbacks and normalizes them into the internal webhook contract.
/// </summary>
public sealed class ZaloPayWebhookIngressService : IZaloPayWebhookIngressService
{
    private readonly HttpClient _httpClient;
    private readonly PaymentProviderGatewayOptions _gatewayOptions;
    private readonly PaymentWebhookSecurityOptions _webhookSecurityOptions;
    private readonly ILogger<ZaloPayWebhookIngressService> _logger;

    public ZaloPayWebhookIngressService(
        HttpClient httpClient,
        IOptions<PaymentProviderGatewayOptions> gatewayOptions,
        IOptions<PaymentWebhookSecurityOptions> webhookSecurityOptions,
        ILogger<ZaloPayWebhookIngressService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _gatewayOptions = gatewayOptions?.Value ?? throw new ArgumentNullException(nameof(gatewayOptions));
        _webhookSecurityOptions = webhookSecurityOptions?.Value ?? throw new ArgumentNullException(nameof(webhookSecurityOptions));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ProcessPaymentWebhookRequest> NormalizeAsync(
        JsonElement payload,
        CancellationToken cancellationToken = default)
    {
        var options = _gatewayOptions.ZaloPay;
        if (!options.Enabled)
        {
            throw new InvalidOperationException("ZaloPay gateway is disabled by configuration.");
        }

        var envelope = JsonSerializer.Deserialize<ZaloPayCallbackEnvelope>(payload.GetRawText(), JsonOptions)
            ?? throw new InvalidOperationException("ZaloPay callback payload is required.");

        if (string.IsNullOrWhiteSpace(envelope.Data) || string.IsNullOrWhiteSpace(envelope.Mac))
        {
            throw new InvalidOperationException("ZaloPay callback must include data and mac.");
        }

        VerifyCallbackMac(options.Key2, envelope.Data, envelope.Mac);

        var callbackData = JsonSerializer.Deserialize<ZaloPayCallbackData>(envelope.Data, JsonOptions)
            ?? throw new InvalidOperationException("ZaloPay callback data is invalid.");

        if (string.IsNullOrWhiteSpace(callbackData.AppTransId))
        {
            throw new InvalidOperationException("ZaloPay callback app_trans_id is required.");
        }

        var intentCode = ExtractIntentCode(callbackData.EmbedData);
        var queryResponse = await QueryOrderAsync(options, callbackData.AppTransId.Trim(), cancellationToken).ConfigureAwait(false);
        var transactionStatus = MapTransactionStatus(
            queryResponse.ReturnCode,
            queryResponse.SubReturnCode,
            queryResponse.IsProcessing);
        var providerTransactionId = queryResponse.ZpTransId?.ToString(CultureInfo.InvariantCulture)
            ?? callbackData.ZpTransId?.ToString(CultureInfo.InvariantCulture)
            ?? callbackData.AppTransId.Trim();

        var request = new ProcessPaymentWebhookRequest
        {
            EventId = BuildEventId(queryResponse.ZpTransId, callbackData.ZpTransId, callbackData.AppTransId),
            IntentCode = intentCode,
            TransactionStatus = transactionStatus.ToString(),
            ProviderTransactionId = providerTransactionId,
            Payload = payload.GetRawText(),
            OccurredAtUtc = callbackData.ServerTime.HasValue
                ? DateTimeOffset.FromUnixTimeMilliseconds(callbackData.ServerTime.Value).UtcDateTime
                : DateTime.UtcNow
        };

        request.SignatureHash = BuildInternalSignature(request);

        _logger.LogInformation(
            "Normalized ZaloPay callback: appTransId={AppTransId}, providerTransactionId={ProviderTransactionId}, status={Status}",
            callbackData.AppTransId,
            providerTransactionId,
            request.TransactionStatus);

        return request;
    }

    private async Task<ZaloPayQueryOrderResponse> QueryOrderAsync(
        ZaloPayGatewayOptions options,
        string appTransId,
        CancellationToken cancellationToken)
    {
        var dataToSign = $"{options.AppId}|{appTransId}|{options.Key1}";
        var mac = ComputeMac(options.Key1, dataToSign);

        var formData = new Dictionary<string, string>
        {
            ["app_id"] = options.AppId.ToString(CultureInfo.InvariantCulture),
            ["app_trans_id"] = appTransId,
            ["mac"] = mac
        };

        using var content = new FormUrlEncodedContent(formData);
        using var response = await _httpClient
            .PostAsync(options.QueryOrderPath, content, cancellationToken)
            .ConfigureAwait(false);

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"ZaloPay query order failed with status {(int)response.StatusCode}: {responseBody}");
        }

        var providerResponse = JsonSerializer.Deserialize<ZaloPayQueryOrderResponse>(responseBody, JsonOptions)
            ?? throw new InvalidOperationException("ZaloPay query order returned an empty response.");

        return providerResponse;
    }

    private static string ExtractIntentCode(string? embedData)
    {
        if (string.IsNullOrWhiteSpace(embedData))
        {
            throw new InvalidOperationException("ZaloPay callback embed_data is required to resolve intentCode.");
        }

        var embedDataObject = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(embedData, JsonOptions)
            ?? throw new InvalidOperationException("ZaloPay callback embed_data is invalid.");

        if (!embedDataObject.TryGetValue("intentCode", out var intentCodeElement) ||
            intentCodeElement.ValueKind != JsonValueKind.String ||
            string.IsNullOrWhiteSpace(intentCodeElement.GetString()))
        {
            throw new InvalidOperationException("ZaloPay callback embed_data.intentCode is required.");
        }

        return intentCodeElement.GetString()!.Trim();
    }

    private string BuildInternalSignature(ProcessPaymentWebhookRequest request)
    {
        if (!_webhookSecurityOptions.Enabled || !_webhookSecurityOptions.RequireSignature)
        {
            return "provider-verified";
        }

        if (string.IsNullOrWhiteSpace(_webhookSecurityOptions.SharedSecret))
        {
            throw new InvalidOperationException("Payment webhook signature secret is not configured.");
        }

        var canonicalMessage = PaymentWebhookSignatureHelper.BuildCanonicalMessage(PaymentProviderType.ZaloPay, request);
        return PaymentWebhookSignatureHelper.ComputeSignature(_webhookSecurityOptions.SharedSecret, canonicalMessage);
    }

    private static string BuildEventId(long? queryZpTransId, long? callbackZpTransId, string appTransId)
    {
        var providerId = queryZpTransId ?? callbackZpTransId;
        return providerId.HasValue
            ? $"zalopay-{providerId.Value.ToString(CultureInfo.InvariantCulture)}"
            : $"zalopay-{appTransId.Trim()}";
    }

    private static PaymentTransactionStatus MapTransactionStatus(
        int returnCode,
        int? subReturnCode,
        bool isProcessing)
    {
        if (returnCode == 1)
        {
            return PaymentTransactionStatus.Completed;
        }

        if (returnCode == 2)
        {
            // ZaloPay -54 means payment window expired.
            if (subReturnCode == -54)
            {
                return PaymentTransactionStatus.Expired;
            }

            return PaymentTransactionStatus.Failed;
        }

        if (returnCode == 3 || isProcessing)
        {
            return PaymentTransactionStatus.InProgress;
        }

        return PaymentTransactionStatus.Failed;
    }

    private static void VerifyCallbackMac(string key2, string data, string providedMac)
    {
        var expectedMac = ComputeMac(key2, data);
        var providedBytes = Encoding.UTF8.GetBytes(providedMac.Trim().ToUpperInvariant());
        var expectedBytes = Encoding.UTF8.GetBytes(expectedMac.ToUpperInvariant());

        if (providedBytes.Length != expectedBytes.Length ||
            !CryptographicOperations.FixedTimeEquals(providedBytes, expectedBytes))
        {
            throw new PaymentWebhookSignatureValidationException("Webhook signature is invalid.");
        }
    }

    private static string ComputeMac(string secret, string data)
    {
        var keyBytes = Encoding.UTF8.GetBytes(secret);
        var dataBytes = Encoding.UTF8.GetBytes(data);

        using var hmac = new HMACSHA256(keyBytes);
        var hash = hmac.ComputeHash(dataBytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private sealed class ZaloPayCallbackEnvelope
    {
        [JsonPropertyName("data")]
        public string Data { get; set; } = string.Empty;

        [JsonPropertyName("mac")]
        public string Mac { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public int Type { get; set; }
    }

    private sealed class ZaloPayCallbackData
    {
        [JsonPropertyName("app_trans_id")]
        public string AppTransId { get; set; } = string.Empty;

        [JsonPropertyName("embed_data")]
        public string? EmbedData { get; set; }

        [JsonPropertyName("zp_trans_id")]
        public long? ZpTransId { get; set; }

        [JsonPropertyName("server_time")]
        public long? ServerTime { get; set; }
    }

    private sealed class ZaloPayQueryOrderResponse
    {
        [JsonPropertyName("return_code")]
        public int ReturnCode { get; set; }

        [JsonPropertyName("return_message")]
        public string ReturnMessage { get; set; } = string.Empty;

        [JsonPropertyName("sub_return_code")]
        public int? SubReturnCode { get; set; }

        [JsonPropertyName("is_processing")]
        public bool IsProcessing { get; set; }

        [JsonPropertyName("zp_trans_id")]
        public long? ZpTransId { get; set; }
    }
}