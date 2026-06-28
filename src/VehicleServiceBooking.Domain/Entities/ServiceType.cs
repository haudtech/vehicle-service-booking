namespace VehicleServiceBooking.Domain.Entities;

/// <summary>
/// ServiceType entity representing a type of vehicle service
/// Configuration for properties is in ApplicationDbContext.OnModelCreating()
/// </summary>
public class ServiceType : BaseEntity
{

    /// <summary>
    /// Service type name (max 100 characters)
    /// Examples: Oil Change, Brake Service, Tire Rotation
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Expected duration of service in minutes
    /// Range: 30 to 480 minutes (30 min to 8 hours)
    /// </summary>
    public int DurationMinutes { get; set; }

    /// <summary>
    /// Base price for this service type.
    /// Stored as a monetary amount in the system default currency.
    /// </summary>
    public decimal Price { get; set; }

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