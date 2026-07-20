using System.ComponentModel.DataAnnotations;
using VehicleServiceBooking.Domain.Enums;

namespace VehicleServiceBooking.Domain.Entities;

/// <summary>
/// Static lookup table for payment order lifecycle statuses.
/// </summary>
public class PaymentIntentStatusLookup : BaseEntity
{
    [Required]
    public PaymentIntentStatus Status { get; set; }

    [MaxLength(50)]
    [Required]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Description { get; set; }

    public ICollection<PaymentOrder> PaymentOrders { get; set; } = new List<PaymentOrder>();
}