using Microsoft.Extensions.Logging;
using VehicleServiceBooking.Notification.Functions.Models;

namespace VehicleServiceBooking.Notification.Functions.Services;

/// <summary>
/// Development-safe Zalo sender that logs outbound Zalo payload.
/// </summary>
public sealed class LoggingZaloSender : IZaloSender
{
    private readonly ILogger<LoggingZaloSender> _logger;

    public LoggingZaloSender(ILogger<LoggingZaloSender> logger)
    {
        _logger = logger;
    }

    public Task SendAsync(NotificationMessage message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "ZALO SEND to={ToZaloUserId}, eventType={EventType}, correlationId={CorrelationId}",
            message.ToZaloUserId,
            message.EventType,
            message.CorrelationId);

        _logger.LogDebug("Zalo content: {Content}", message.Content);
        return Task.CompletedTask;
    }
}
