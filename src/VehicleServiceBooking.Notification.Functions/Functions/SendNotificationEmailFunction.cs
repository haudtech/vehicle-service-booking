using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using VehicleServiceBooking.Notification.Functions.Common.Enums;
using VehicleServiceBooking.Notification.Functions.Models;
using VehicleServiceBooking.Notification.Functions.Services;

namespace VehicleServiceBooking.Notification.Functions.Functions;

/// <summary>
/// Queue-triggered function that sends user notifications to supported channels.
/// </summary>
public sealed class SendNotificationEmailFunction
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly IEmailSender _emailSender;
    private readonly IZaloSender _zaloSender;
    private readonly ILogger<SendNotificationEmailFunction> _logger;

    public SendNotificationEmailFunction(IEmailSender emailSender, IZaloSender zaloSender, ILogger<SendNotificationEmailFunction> logger)
    {
        _emailSender = emailSender;
        _zaloSender = zaloSender;
        _logger = logger;
    }

    [Function("SendNotificationEmail")]
    public async Task Run(
        [QueueTrigger("%NotificationQueueName%", Connection = "AzureWebJobsStorage")]
        string queuePayload,
        FunctionContext context,
        CancellationToken cancellationToken)
    {
        NotificationMessage? message;
        try
        {
            message = JsonSerializer.Deserialize<NotificationMessage>(queuePayload, JsonOptions);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Skipping notification message because payload is invalid JSON. Payload={Payload}", queuePayload);
            return;
        }

        if (message is null)
        {
            _logger.LogError("Skipping notification message because payload could not be deserialized. Payload={Payload}", queuePayload);
            return;
        }

        var destinationChannel = NotificationChannelExtensions.ParseWireValueOrDefault(message.DestinationChannel);

        if (destinationChannel == NotificationChannel.Zalo)
        {
            if (string.IsNullOrWhiteSpace(message.ToZaloUserId))
            {
                _logger.LogWarning(
                    "Skipping notification message because ToZaloUserId is missing. EventType={EventType}, CorrelationId={CorrelationId}",
                    message.EventType,
                    message.CorrelationId);
                return;
            }

            try
            {
                await _zaloSender.SendAsync(message, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Zalo notification delivery failed. EventType={EventType}, ToZaloUserId={ToZaloUserId}, CorrelationId={CorrelationId}",
                    message.EventType,
                    message.ToZaloUserId,
                    message.CorrelationId);
                throw;
            }

            _logger.LogInformation(
                "Processed Zalo notification message. EventType={EventType}, ToZaloUserId={ToZaloUserId}, CorrelationId={CorrelationId}",
                message.EventType,
                message.ToZaloUserId,
                message.CorrelationId);
            return;
        }

        if (destinationChannel == NotificationChannel.AuthenticatorApp)
        {
            _logger.LogWarning(
                "Skipping notification message because authenticator channel has no outbound sender. EventType={EventType}, CorrelationId={CorrelationId}",
                message.EventType,
                message.CorrelationId);
            return;
        }

        if (string.IsNullOrWhiteSpace(message.ToEmail))
        {
            _logger.LogWarning(
                "Skipping notification message because ToEmail is missing. EventType={EventType}, CorrelationId={CorrelationId}",
                message.EventType,
                message.CorrelationId);
            return;
        }

        if (string.IsNullOrWhiteSpace(message.Subject))
        {
            _logger.LogWarning(
                "Skipping notification message because Subject is missing. EventType={EventType}, ToEmail={ToEmail}, CorrelationId={CorrelationId}",
                message.EventType,
                message.ToEmail,
                message.CorrelationId);
            return;
        }

        try
        {
            await _emailSender.SendAsync(message, cancellationToken);
        }
        catch (Exception ex)
        {
            // Re-throw for retry/dead-letter behavior on transient downstream failures.
            _logger.LogError(
                ex,
                "Notification delivery failed. EventType={EventType}, ToEmail={ToEmail}, CorrelationId={CorrelationId}",
                message.EventType,
                message.ToEmail,
                message.CorrelationId);
            throw;
        }

        _logger.LogInformation(
            "Processed notification message. EventType={EventType}, ToEmail={ToEmail}, CorrelationId={CorrelationId}",
            message.EventType,
            message.ToEmail,
            message.CorrelationId);
    }
}
