using System.ComponentModel.DataAnnotations;
using VehicleServiceBooking.Domain.Enums;

namespace VehicleServiceBooking.Domain.Entities;

/// <summary>
/// Static lookup table for order-level payment lifecycle statuses.
/// </summary>
public class OrderPaymentStatusLookup : BaseEntity
{
    [Required]
    public OrderPaymentStatus Status { get; set; }

    [MaxLength(50)]
    [Required]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Description { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}