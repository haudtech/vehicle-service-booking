namespace VehicleServiceBooking.Notification.Functions.Models;

/// <summary>
/// Queue message contract produced by Auth service for email notification delivery.
/// </summary>
public sealed class NotificationMessage
{
    public string EventType { get; set; } = string.Empty;
    public string ToEmail { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? HtmlContent { get; set; }
    public string CorrelationId { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public DateTime OccurredAtUtc { get; set; }
}
