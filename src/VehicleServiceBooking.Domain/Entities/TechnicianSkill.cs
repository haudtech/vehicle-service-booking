namespace VehicleServiceBooking.Domain.Entities;

/// <summary>
/// TechnicianSkill represents the many-to-many relationship between technicians and service types
/// A technician can perform multiple service types, and a service type can be performed by multiple technicians
/// </summary>
public class TechnicianSkill : BaseEntity
{

    /// <summary>
    /// Reference to the technician
    /// </summary>
    public Guid TechnicianId { get; set; }

    /// <summary>
    /// Navigation property to the technician
    /// </summary>
    public Technician Technician { get; set; } = null!;

    /// <summary>
    /// Reference to the service type
    /// </summary>
    public Guid ServiceTypeId { get; set; }

    /// <summary>
    /// Navigation property to the service type
    /// </summary>
    public ServiceType ServiceType { get; set; } = null!;
}