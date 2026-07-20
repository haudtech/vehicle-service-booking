using VehicleServiceBooking.Domain.Enums;

namespace VehicleServiceBooking.Domain.Entities;

/// <summary>
/// Represents a single financial transaction moving funds in or out of the system.
/// </summary>
public class PaymentTransaction : BaseEntity
{
    public string TransactionCode { get; set; } = string.Empty;

    public string FromAccount { get; set; } = string.Empty;

    public string ToAccount { get; set; } = string.Empty;

    public PaymentTransactionDirection Direction { get; set; }

    public decimal Amount { get; set; }

    public Guid CurrencyId { get; set; }

    public Guid StatusId { get; set; }

    public DateTime TransactionAtUtc { get; set; }

    public CurrencyLookup Currency { get; set; } = null!;

    public PaymentTransactionStatusLookup Status { get; set; } = null!;

    public PaymentOrder? PaymentOrder { get; set; }
}
