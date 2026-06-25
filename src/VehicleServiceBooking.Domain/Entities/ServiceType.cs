using System.ComponentModel.DataAnnotations;

namespace VehicleServiceBooking.Domain.Entities;

/// <summary>
/// ServiceType entity representing a type of vehicle service
/// </summary>
public class ServiceType : BaseEntity
{

    /// <summary>
    /// Service type name (max 100 characters)
    /// Examples: Oil Change, Brake Service, Tire Rotation
    /// </summary>
    [MaxLength(100)]
    [Required]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Expected duration of service in minutes
    /// Range: 15 to 480 minutes (15 min to 8 hours)
    /// </summary>
    [Range(30, 480)]
    [Required]
    public int DurationMinutes { get; set; }

    /// <summary>
    /// Technicians who can perform this service
    /// </summary>
    public ICollection<TechnicianSkill> TechnicianSkills { get; set; }
        = new List<TechnicianSkill>();

    /// <summary>
    /// Services of this type scheduled in appointments
    /// </summary>
    public ICollection<Service> Services { get; set; }
        = new List<Service>();
}