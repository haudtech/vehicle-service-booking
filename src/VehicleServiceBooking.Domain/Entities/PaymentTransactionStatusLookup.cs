using VehicleServiceBooking.Domain.Enums;

namespace VehicleServiceBooking.Domain.Entities;

/// <summary>
/// Static lookup table for payment transaction lifecycle statuses.
/// </summary>
public class PaymentTransactionStatusLookup : BaseEntity
{
    public PaymentTransactionStatus Status { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
}
