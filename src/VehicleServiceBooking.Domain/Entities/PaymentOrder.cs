namespace VehicleServiceBooking.Domain.Entities;

/// <summary>
/// Payment record linked to an Order and one transaction.
/// </summary>
public class PaymentOrder : BaseEntity
{
    public Guid OrderId { get; set; }

    public Guid PaymentProviderId { get; set; }

    public Guid PaymentMethodId { get; set; }

    public Guid StatusId { get; set; }

    public PaymentIntentStatusLookup Status { get; set; } = null!;

    public string IntentCode { get; set; } = string.Empty;

    public string? CheckoutUrl { get; set; }

    public DateTime? ExpiresAtUtc { get; set; }

    public string? LastProviderEventId { get; set; }

    public DateTime? LastProviderEventAtUtc { get; set; }

    public Guid? PaymentTransactionId { get; set; }

    public DateTime OrderedAtUtc { get; set; }

    public Order Order { get; set; } = null!;

    public PaymentProviderLookup PaymentProvider { get; set; } = null!;

    public PaymentMethodLookup PaymentMethod { get; set; } = null!;

    public PaymentTransaction? PaymentTransaction { get; set; }
}
