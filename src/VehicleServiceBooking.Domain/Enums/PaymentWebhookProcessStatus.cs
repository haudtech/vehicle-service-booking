namespace VehicleServiceBooking.Domain.Enums;

/// <summary>
/// Processing status for an inbound payment webhook event.
/// </summary>
public enum PaymentWebhookProcessStatus
{
    Received = 1,
    Processed = 2,
    Ignored = 3,
    Failed = 4
}