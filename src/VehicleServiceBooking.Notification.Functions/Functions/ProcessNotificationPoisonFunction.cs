using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using VehicleServiceBooking.Notification.Functions.Models;

namespace VehicleServiceBooking.Notification.Functions.Functions;

/// <summary>
/// Queue-triggered function that records diagnostic details for messages moved to the poison queue.
/// </summary>
public sealed class ProcessNotificationPoisonFunction
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly ILogger<ProcessNotificationPoisonFunction> _logger;

    public ProcessNotificationPoisonFunction(ILogger<ProcessNotificationPoisonFunction> logger)
    {
        _logger = logger;
    }

    [Function("ProcessNotificationPoison")]
    public void Run(
        [QueueTrigger("%NotificationPoisonQueueName%", Connection = "AzureWebJobsStorage")]
        string poisonPayload)
    {
        if (string.IsNullOrWhiteSpace(poisonPayload))
        {
            _logger.LogWarning("Poison queue message received with empty payload.");
            return;
        }

        try
        {
            var message = JsonSerializer.Deserialize<NotificationMessage>(poisonPayload, JsonOptions);
            if (message is null)
            {
                _logger.LogError(
                    "Poison queue message could not be deserialized to NotificationMessage. Payload={Payload}",
                    poisonPayload);
                return;
            }

            _logger.LogError(
                "Notification moved to poison queue. EventType={EventType}, ToEmail={ToEmail}, CorrelationId={CorrelationId}, Source={Source}, OccurredAtUtc={OccurredAtUtc}",
                message.EventType,
                message.ToEmail,
                message.CorrelationId,
                message.Source,
                message.OccurredAtUtc);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Poison queue message contains invalid JSON. Payload={Payload}", poisonPayload);
        }
    }
}
