using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using VehicleServiceBooking.Application.DTOs;
using VehicleServiceBooking.Domain.Enums;

namespace VehicleServiceBooking.Application.Helpers;

/// <summary>
/// Utility methods for canonicalizing and signing payment webhook payloads.
/// </summary>
public static class PaymentWebhookSignatureHelper
{
    public static string BuildCanonicalMessage(PaymentProviderType providerType, ProcessPaymentWebhookRequest request)
    {
        var occurredAt = request.OccurredAtUtc?.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture) ?? string.Empty;
        var providerTransactionId = request.ProviderTransactionId?.Trim() ?? string.Empty;

        return string.Join(
            "|",
            providerType.ToString(),
            request.EventId.Trim(),
            request.IntentCode.Trim(),
            request.TransactionStatus.Trim(),
            providerTransactionId,
            occurredAt);
    }

    public static string ComputeSignature(string secret, string canonicalMessage)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(canonicalMessage));
        return Convert.ToHexString(hash);
    }
}