namespace VehicleServiceBooking.Domain.Entities;

/// <summary>
/// Dedupe and processing inbox for provider webhook events.
/// </summary>
public class PaymentWebhookInbox : BaseEntity
{
    public Guid PaymentProviderId { get; set; }

    public string EventId { get; set; } = string.Empty;

    public string SignatureHash { get; set; } = string.Empty;

    public string Payload { get; set; } = string.Empty;

    public Guid ProcessStatusId { get; set; }

    public PaymentWebhookProcessStatusLookup ProcessStatus { get; set; } = null!;

    public DateTime ReceivedAtUtc { get; set; }

    public DateTime? ProcessedAtUtc { get; set; }

    public string? ErrorMessage { get; set; }

    public PaymentProviderLookup PaymentProvider { get; set; } = null!;
}