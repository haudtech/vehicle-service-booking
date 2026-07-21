namespace VehicleServiceBooking.Domain.Enums;

/// <summary>
/// Overall payment lifecycle status for an order.
/// </summary>
public enum OrderPaymentStatus
{
    Pending = 1,
    Paid = 2,
    Failed = 3,
    Expired = 4,
    Cancelled = 5,
    Refunded = 6
}