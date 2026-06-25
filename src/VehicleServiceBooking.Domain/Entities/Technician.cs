using System.ComponentModel.DataAnnotations;

namespace VehicleServiceBooking.Domain.Entities;

/// <summary>
/// Technician entity representing a service technician at a dealership
/// </summary>
public class Technician : BaseEntity
{

    /// <summary>
    /// Reference to the dealership where this technician works
    /// </summary>
    public Guid DealershipId { get; set; }

    /// <summary>
    /// Navigation property to the dealership
    /// </summary>
    public Dealership Dealership { get; set; } = null!;

    /// <summary>
    /// Technician's first name (max 100 characters)
    /// </summary>
    [MaxLength(100)]
    [Required]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Technician's last name (max 100 characters)
    /// </summary>
    [MaxLength(100)]
    [Required]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Work schedules for this technician
    /// </summary>
    public ICollection<TechnicianSchedule> Schedules { get; set; }
        = new List<TechnicianSchedule>();

    /// <summary>
    /// Services this technician is qualified to perform
    /// </summary>
    public ICollection<TechnicianSkill> Skills { get; set; } 
        = new List<TechnicianSkill>();

    /// <summary>
    /// Services assigned to this technician
    /// </summary>
    public ICollection<Service> Services { get; set; }
        = new List<Service>();
}