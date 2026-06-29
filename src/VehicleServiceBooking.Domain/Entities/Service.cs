namespace VehicleServiceBooking.Domain.Entities;

/// <summary>
/// Service represents a service type within an appointment.
/// This junction table enables N:M relationship between Appointment and ServiceType,
/// allowing customers to book multiple services in a single appointment.
/// 
/// TIMING MODEL:
/// - EstimatedStartTimeSlotId/EstimatedEndTimeSlotId: References static TimeSlot (query-optimized)
/// - ActualStartTime/ActualEndTime: Real-world execution times (arbitrary precision)
/// - EstimatedStartTime/EstimatedEndTime: Computed properties for backward compatibility
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
    /// Foreign key to the TimeSlot for estimated start time (grid-based scheduling)
    /// References TimeSlot.Id to maintain alignment with 30-min slot grid
    /// </summary>
    public Guid? EstimatedStartTimeSlotId { get; set; }

    /// <summary>
    /// Foreign key to the TimeSlot for estimated end time (grid-based scheduling)
    /// References TimeSlot.Id to maintain alignment with 30-min slot grid
    /// </summary>
    public Guid? EstimatedEndTimeSlotId { get; set; }

    /// <summary>
    /// Persisted booking date snapshot used for database-level overlap constraints.
    /// Mirrors Appointment.AppointmentDate at write time.
    /// </summary>
    public DateOnly BookingDate { get; set; }

    /// <summary>
    /// Persisted start slot sequence used for database-level overlap constraints.
    /// </summary>
    public int EstimatedStartSlotSequence { get; set; }

    /// <summary>
    /// Persisted exclusive end slot sequence used for overlap-safe range comparison.
    /// </summary>
    public int EstimatedEndSlotSequenceExclusive { get; set; }

    /// <summary>
    /// Sequential order of services within the appointment (1-based)
    /// </summary>
    public int SequenceOrder { get; set; }

    /// <summary>
    /// Actual start time when the service began (arbitrary precision for real-world execution)
    /// </summary>
    public DateTime? ActualStartTime { get; set; }

    /// <summary>
    /// Actual end time when the service was completed (arbitrary precision for real-world execution)
    /// </summary>
    public DateTime? ActualEndTime { get; set; }

    /// <summary>
    /// Notes for the service (observations, issues, recommendations)
    /// </summary>
    public string Notes { get; set; } = string.Empty;

    // ============================================
    // Computed Properties (Backward Compatibility)
    // ============================================

    /// <summary>
    /// Estimated start time computed from TimeSlot reference and Appointment date.
    /// This is NOT persisted in the database, but computed on-demand from:
    /// - Appointment.AppointmentDate (the date)
    /// - EstimatedStartTimeSlot.SlotStartTime (the time within the day)
    /// 
    /// Returns null if either EstimatedStartTimeSlot or Appointment is not loaded.
    /// </summary>
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public DateTime? EstimatedStartTime =>
        EstimatedStartTimeSlot != null && Appointment != null
            ? Appointment.AppointmentDate.ToDateTime(EstimatedStartTimeSlot.SlotStartTime)
            : null;

    /// <summary>
    /// Estimated end time computed from TimeSlot reference and Appointment date.
    /// This is NOT persisted in the database, but computed on-demand from:
    /// - Appointment.AppointmentDate (the date)
    /// - EstimatedEndTimeSlot.SlotEndTime (the time within the day)
    /// 
    /// Returns null if either EstimatedEndTimeSlot or Appointment is not loaded.
    /// </summary>
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public DateTime? EstimatedEndTime =>
        EstimatedEndTimeSlot != null && Appointment != null
            ? Appointment.AppointmentDate.ToDateTime(EstimatedEndTimeSlot.SlotEndTime)
            : null;

    // ============================================
    // Navigation Properties
    // ============================================

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

    /// <summary>
    /// Navigation property to the TimeSlot for estimated START time.
    /// References the static TimeSlot table (30-min grid).
    /// When loaded, EstimatedStartTime computed property will use this.
    /// </summary>
    public TimeSlot? EstimatedStartTimeSlot { get; set; }

    /// <summary>
    /// Navigation property to the TimeSlot for estimated END time.
    /// References the static TimeSlot table (30-min grid).
    /// When loaded, EstimatedEndTime computed property will use this.
    /// </summary>
    public TimeSlot? EstimatedEndTimeSlot { get; set; }
}
