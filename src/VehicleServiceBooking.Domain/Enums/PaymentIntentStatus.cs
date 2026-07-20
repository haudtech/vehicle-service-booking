namespace VehicleServiceBooking.Domain.Enums;

/// <summary>
/// Lifecycle status for a payment intent/order record.
/// </summary>
public enum PaymentIntentStatus
{
    Initiated = 1,
    Redirected = 2,
    Paid = 3,
    Failed = 4,
    Expired = 5,
    Cancelled = 6
}