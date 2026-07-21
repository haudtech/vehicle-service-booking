using VehicleServiceBooking.Domain.Enums;

namespace VehicleServiceBooking.Domain.Entities;

/// <summary>
/// Represents a single financial transaction moving funds in or out of the system.
/// </summary>
public class PaymentTransaction : BaseEntity
{
    public string TransactionCode { get; set; } = string.Empty;

    public Guid PaymentProviderId { get; set; }

    public Guid PaymentMethodId { get; set; }

    public string FromAccount { get; set; } = string.Empty;

    public string ToAccount { get; set; } = string.Empty;

    public PaymentTransactionDirection Direction { get; set; }

    public decimal Amount { get; set; }

    public Guid CurrencyId { get; set; }

    public Guid StatusId { get; set; }

    public string? ProviderTransactionId { get; set; }

    public string? ProviderEventId { get; set; }

    public string? RawProviderPayload { get; set; }

    public DateTime? CompletedAtUtc { get; set; }

    public DateTime? FailedAtUtc { get; set; }

    public string? FailureCode { get; set; }

    public string? FailureMessage { get; set; }

    public DateTime TransactionAtUtc { get; set; }

    public CurrencyLookup Currency { get; set; } = null!;

    public PaymentProviderLookup PaymentProvider { get; set; } = null!;

    public PaymentMethodLookup PaymentMethod { get; set; } = null!;

    public PaymentTransactionStatusLookup Status { get; set; } = null!;

    public PaymentOrder? PaymentOrder { get; set; }
}
