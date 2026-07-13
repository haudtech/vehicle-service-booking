using VehicleServiceBooking.Auth.Models.Notifications;

namespace VehicleServiceBooking.Auth.Services;

/// <summary>
/// Publishes notification messages to downstream delivery infrastructure.
/// </summary>
public interface INotificationPublisher
{
    /// <summary>
    /// Publishes a notification message asynchronously.
    /// </summary>
    /// <param name="message">Message to publish.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task PublishAsync(NotificationMessage message, CancellationToken cancellationToken = default);
}
