namespace VehicleServiceBooking.Domain.Entities;

/// <summary>
/// Payment record linked to an Order and one transaction.
/// </summary>
public class PaymentOrder : BaseEntity
{
    public Guid OrderId { get; set; }

    public Guid PaymentTransactionId { get; set; }

    public DateTime OrderedAtUtc { get; set; }

    public Order Order { get; set; } = null!;

    public PaymentTransaction PaymentTransaction { get; set; } = null!;
}
