using VehicleServiceBooking.Auth.Common.Enums;

namespace VehicleServiceBooking.Auth.Models.Notifications;

/// <summary>
/// Message contract published by Auth service to notification queue.
/// </summary>
public sealed class NotificationMessage
{
    /// <summary>
    /// Gets or sets destination channel (email or zalo).
    /// </summary>
    public string DestinationChannel { get; set; } = NotificationChannel.Email.ToWireValue();

    /// <summary>
    /// Gets or sets event type emitted by the source service.
    /// </summary>
    public string EventType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets destination user email.
    /// </summary>
    public string ToEmail { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets destination zalo user identifier for ZNS delivery.
    /// </summary>
    public string ToZaloUserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets email subject.
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets email body content.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets optional HTML body content.
    /// </summary>
    public string? HtmlContent { get; set; }

    /// <summary>
    /// Gets or sets correlation identifier from request pipeline.
    /// </summary>
    public string CorrelationId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets source service identifier.
    /// </summary>
    public string Source { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets UTC timestamp when the message was produced.
    /// </summary>
    public DateTime OccurredAtUtc { get; set; }
}
