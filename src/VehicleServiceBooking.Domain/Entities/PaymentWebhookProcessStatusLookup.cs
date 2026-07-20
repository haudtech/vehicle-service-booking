using System.ComponentModel.DataAnnotations;
using VehicleServiceBooking.Domain.Enums;

namespace VehicleServiceBooking.Domain.Entities;

/// <summary>
/// Static lookup table for inbound payment webhook processing statuses.
/// </summary>
public class PaymentWebhookProcessStatusLookup : BaseEntity
{
    [Required]
    public PaymentWebhookProcessStatus Status { get; set; }

    [MaxLength(50)]
    [Required]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Description { get; set; }

    public ICollection<PaymentWebhookInbox> PaymentWebhookInboxes { get; set; } = new List<PaymentWebhookInbox>();
}