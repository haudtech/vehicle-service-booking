using VehicleServiceBooking.Auth.Models.Notifications;

namespace VehicleServiceBooking.Auth.Services;

/// <summary>
/// No-op notification publisher used when notification integration is disabled.
/// </summary>
public sealed class NoOpNotificationPublisher : INotificationPublisher
{
    /// <inheritdoc />
    public Task PublishAsync(NotificationMessage message, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
