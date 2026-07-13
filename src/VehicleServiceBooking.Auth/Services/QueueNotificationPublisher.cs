using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.Extensions.Logging;
using VehicleServiceBooking.Auth.Models.Notifications;

namespace VehicleServiceBooking.Auth.Services;

/// <summary>
/// Publishes notification messages to Azure Storage Queue.
/// </summary>
public sealed class QueueNotificationPublisher : INotificationPublisher
{
    private readonly QueueClient _queueClient;
    private readonly ILogger<QueueNotificationPublisher> _logger;
    private readonly SemaphoreSlim _queueEnsureLock = new(1, 1);
    private bool _queueEnsured;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueueNotificationPublisher"/> class.
    /// </summary>
    public QueueNotificationPublisher(QueueClient queueClient, ILogger<QueueNotificationPublisher> logger)
    {
        _queueClient = queueClient;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task PublishAsync(NotificationMessage message, CancellationToken cancellationToken = default)
    {
        await EnsureQueueExistsAsync(cancellationToken);

        var payload = JsonSerializer.Serialize(message);
        await _queueClient.SendMessageAsync(payload, cancellationToken: cancellationToken);

        _logger.LogInformation(
            "Published notification message. EventType={EventType}, ToEmail={ToEmail}, CorrelationId={CorrelationId}",
            message.EventType,
            message.ToEmail,
            message.CorrelationId);
    }

    private async Task EnsureQueueExistsAsync(CancellationToken cancellationToken)
    {
        if (_queueEnsured)
        {
            return;
        }

        await _queueEnsureLock.WaitAsync(cancellationToken);
        try
        {
            if (_queueEnsured)
            {
                return;
            }

            await _queueClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
            _queueEnsured = true;

            _logger.LogInformation("Ensured notification queue exists: {QueueName}", _queueClient.Name);
        }
        finally
        {
            _queueEnsureLock.Release();
        }
    }
}
