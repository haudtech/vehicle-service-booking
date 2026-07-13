using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VehicleServiceBooking.Notification.Functions.Configuration;
using VehicleServiceBooking.Notification.Functions.Models;

namespace VehicleServiceBooking.Notification.Functions.Services;

/// <summary>
/// Development-safe email sender that logs outbound email payload.
/// </summary>
public sealed class LoggingEmailSender : IEmailSender
{
    private readonly EmailSenderOptions _options;
    private readonly ILogger<LoggingEmailSender> _logger;

    public LoggingEmailSender(IOptions<EmailSenderOptions> options, ILogger<LoggingEmailSender> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public Task SendAsync(NotificationMessage message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "EMAIL SEND (provider={Provider}) to={ToEmail}, from={FromAddress}, subject={Subject}, eventType={EventType}, correlationId={CorrelationId}",
            _options.Provider,
            message.ToEmail,
            _options.FromAddress,
            message.Subject,
            message.EventType,
            message.CorrelationId);

        _logger.LogDebug("Email body content: {Content}", message.Content);
        return Task.CompletedTask;
    }
}
