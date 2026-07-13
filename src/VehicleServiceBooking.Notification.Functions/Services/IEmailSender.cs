using VehicleServiceBooking.Notification.Functions.Models;

namespace VehicleServiceBooking.Notification.Functions.Services;

/// <summary>
/// Sends email for notification events.
/// </summary>
public interface IEmailSender
{
    /// <summary>
    /// Sends an email derived from a notification message.
    /// </summary>
    Task SendAsync(NotificationMessage message, CancellationToken cancellationToken = default);
}
