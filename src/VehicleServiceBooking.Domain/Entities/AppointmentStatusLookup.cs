using System.ComponentModel.DataAnnotations;
using VehicleServiceBooking.Domain.Enums;

namespace VehicleServiceBooking.Domain.Entities;

/// <summary>
/// AppointmentStatusLookup entity representing the static lookup table for appointment statuses
/// </summary>
public class AppointmentStatusLookup : BaseEntity
{
    /// <summary>
    /// Status enum value (Booked, InProgress, Completed, Cancelled)
    /// </summary>
    [Required]
    public AppointmentStatus Status { get; set; }

    /// <summary>
    /// Status display name (max 50 characters)
    /// Examples: Booked, In Progress, Completed, Cancelled
    /// </summary>
    [MaxLength(50)]
    [Required]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Status description (max 200 characters)
    /// </summary>
    [MaxLength(200)]
    public string? Description { get; set; }

    /// <summary>
    /// Collection of appointments with this status
    /// </summary>
    public ICollection<Appointment> Appointments { get; set; }
        = new List<Appointment>();
}
