using System;

namespace VehicleServiceBooking.Application.Models.ViewModels;

/// <summary>
/// Master availability view combining service type requirements with technician and bay availability.
/// This DTO is populated from the ServiceTypeAvailability SQL view.
/// 
/// Used by: AvailabilityService to provide complete availability information for each service type,
/// considering service duration, technician skills, and bay availability.
/// </summary>
public class ServiceTypeAvailabilityView
{
    /// <summary>
    /// The service type ID
    /// </summary>
    public Guid ServiceTypeId { get; set; }

    /// <summary>
    /// Service type name (for display)
    /// </summary>
    public string ServiceTypeName { get; set; } = string.Empty;

    /// <summary>
    /// Duration of this service in minutes
    /// </summary>
    public int DurationMinutes { get; set; }

    /// <summary>
    /// Number of consecutive 30-min slots required for this service
    /// (e.g., 60 minutes = 2 slots)
    /// </summary>
    public int RequiredSlots { get; set; }

    /// <summary>
    /// The starting time slot ID
    /// </summary>
    public Guid TimeSlotId { get; set; }

    /// <summary>
    /// Sequential order of starting slot (1-18)
    /// </summary>
    public int SequenceOrder { get; set; }

    /// <summary>
    /// Start time of the slot (e.g., 08:00)
    /// </summary>
    public TimeOnly SlotStartTime { get; set; }

    /// <summary>
    /// End time of the starting slot (e.g., 08:30)
    /// </summary>
    public TimeOnly SlotEndTime { get; set; }

    /// <summary>
    /// Qualified technician ID
    /// </summary>
    public Guid TechnicianId { get; set; }

    /// <summary>
    /// Technician's first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Technician's last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Available service bay ID
    /// </summary>
    public Guid ServiceBayId { get; set; }

    /// <summary>
    /// Service bay name (for display)
    /// </summary>
    public string ServiceBayName { get; set; } = string.Empty;

    /// <summary>
    /// Dealership ID (for isolation)
    /// </summary>
    public Guid DealershipId { get; set; }

    /// <summary>
    /// Query date (for debugging/auditing)
    /// </summary>
    public DateOnly QueryDate { get; set; }

    /// <summary>
    /// Whether the service can fit starting at this slot
    /// (Checks that N consecutive slots are available for duration)
    /// </summary>
    public bool CanFitService { get; set; }
}
