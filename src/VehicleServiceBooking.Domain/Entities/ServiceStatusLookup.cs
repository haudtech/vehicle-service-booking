using VehicleServiceBooking.Domain.Enums;

namespace VehicleServiceBooking.Domain.Entities;

/// <summary>
/// ServiceStatusLookup represents the available statuses for services within an appointment.
/// This is a static lookup table that provides the mapping between ServiceStatus enum values
/// and their descriptions, used to track the status of individual services.
/// </summary>
public class ServiceStatusLookup : BaseEntity
{

    /// <summary>
    /// The ServiceStatus enum value
    /// </summary>
    public ServiceStatus Status { get; set; }

    /// <summary>
    /// Display name for the status (e.g., "Pending", "In Progress")
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of what this status represents
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property for services with this status
    /// </summary>
    public ICollection<Service> Services { get; set; } = new List<Service>();
}
