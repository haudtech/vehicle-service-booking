using VehicleServiceBooking.Notification.Functions.Models;

namespace VehicleServiceBooking.Notification.Functions.Services;

/// <summary>
/// Sends Zalo notifications for verification and security events.
/// </summary>
public interface IZaloSender
{
    /// <summary>
    /// Sends a notification message to Zalo destination.
    /// </summary>
    Task SendAsync(NotificationMessage message, CancellationToken cancellationToken = default);
}
