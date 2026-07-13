namespace VehicleServiceBooking.Auth.Configuration;

/// <summary>
/// Configuration values for asynchronous notification publishing.
/// </summary>
public sealed class NotificationOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether queue-based notification publishing is enabled.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets the maximum time allowed for a notification publish attempt before it is skipped.
    /// </summary>
    public TimeSpan PublishTimeout { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Gets or sets Azure Storage Queue connection string.
    /// </summary>
    public string QueueConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets target queue name.
    /// </summary>
    public string QueueName { get; set; } = "user-notification-events";

    /// <summary>
    /// Gets or sets producer service name included in published messages.
    /// </summary>
    public string Source { get; set; } = "auth-service";
}
