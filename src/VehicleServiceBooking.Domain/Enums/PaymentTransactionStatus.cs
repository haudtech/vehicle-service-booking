namespace VehicleServiceBooking.Domain.Enums;

/// <summary>
/// Static lifecycle statuses for payment transactions.
/// </summary>
public enum PaymentTransactionStatus
{
    Pending = 1,
    InProgress = 2,
    Completed = 3,
    Failed = 4,
    Expired = 5,
    Cancelled = 6,
    Refunded = 7
}
