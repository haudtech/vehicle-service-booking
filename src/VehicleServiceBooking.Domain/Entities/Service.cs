namespace VehicleServiceBooking.Domain.Entities;

/// <summary>
/// Service represents a service type within an appointment.
/// This junction table enables N:M relationship between Appointment and ServiceType,
/// allowing customers to book multiple services in a single appointment.
/// </summary>
public class Service : BaseEntity
{

    /// <summary>
    /// Foreign key to the Appointment this service belongs to
    /// </summary>
    public Guid AppointmentId { get; set; }

    /// <summary>
    /// Foreign key to the ServiceType being performed
    /// </summary>
    public Guid ServiceTypeId { get; set; }

    /// <summary>
    /// Foreign key to the Technician assigned to this service (optional - assigned later)
    /// </summary>
    public Guid? TechnicianId { get; set; }

    /// <summary>
    /// Foreign key to the ServiceBay where this service is performed (optional - assigned later)
    /// </summary>
    public Guid? ServiceBayId { get; set; }

    /// <summary>
    /// Foreign key to the Dealership (denormalized for query efficiency)
    /// </summary>
    public Guid DealershipId { get; set; }

    /// <summary>
    /// Foreign key to the ServiceStatus lookup
    /// </summary>
    public Guid ServiceStatusId { get; set; }

    /// <summary>
    /// Sequential order of services within the appointment (1-based)
    /// </summary>
    public int SequenceOrder { get; set; }

    /// <summary>
    /// Estimated start time for the service
    /// </summary>
    public DateTime? EstimatedStartTime { get; set; }

    /// <summary>
    /// Estimated end time for the service
    /// </summary>
    public DateTime? EstimatedEndTime { get; set; }

    /// <summary>
    /// Actual start time when the service began
    /// </summary>
    public DateTime? ActualStartTime { get; set; }

    /// <summary>
    /// Actual end time when the service was completed
    /// </summary>
    public DateTime? ActualEndTime { get; set; }

    /// <summary>
    /// Notes for the service (observations, issues, recommendations)
    /// </summary>
    public string Notes { get; set; } = string.Empty;

    // Navigation properties
    /// <summary>
    /// Navigation property to the Appointment this service belongs to
    /// </summary>
    public Appointment Appointment { get; set; } = null!;

    /// <summary>
    /// Navigation property to the ServiceType being performed
    /// </summary>
    public ServiceType ServiceType { get; set; } = null!;

    /// <summary>
    /// Navigation property to the assigned Technician (optional)
    /// </summary>
    public Technician? Technician { get; set; }

    /// <summary>
    /// Navigation property to the ServiceBay (optional)
    /// </summary>
    public ServiceBay? ServiceBay { get; set; }

    /// <summary>
    /// Navigation property to the Dealership
    /// </summary>
    public Dealership Dealership { get; set; } = null!;

    /// <summary>
    /// Navigation property to the ServiceStatus lookup
    /// </summary>
    public ServiceStatusLookup ServiceStatus { get; set; } = null!;
}
