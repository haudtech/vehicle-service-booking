using System;

namespace VehicleServiceBooking.Application.DTOs;

/// <summary>
/// Request payload for inbound payment provider webhook processing.
/// </summary>
public sealed class ProcessPaymentWebhookRequest
{
    public string EventId { get; set; } = string.Empty;

    public string IntentCode { get; set; } = string.Empty;

    public string TransactionStatus { get; set; } = string.Empty;

    public string? ProviderTransactionId { get; set; }

    public string? SignatureHash { get; set; }

    public string? Payload { get; set; }

    public DateTime? OccurredAtUtc { get; set; }
}

/// <summary>
/// Result payload for payment webhook processing.
/// </summary>
public sealed class ProcessPaymentWebhookResponse
{
    public Guid WebhookInboxId { get; set; }

    public string EventId { get; set; } = string.Empty;

    public bool IsDuplicate { get; set; }

    public string ProcessStatus { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
}