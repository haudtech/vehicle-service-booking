using System;

namespace VehicleServiceBooking.Application.Models.ViewModels;

/// <summary>
/// Represents a time slot available for a specific technician on a given date.
/// This DTO is populated from the TechnicianAvailableSlots SQL view.
/// 
/// Used by: AvailabilityService to determine which technicians can perform services
/// in which time slots without conflicts.
/// </summary>
public class TechnicianAvailableSlot
{
    /// <summary>
    /// Reference to the TimeSlot entity
    /// </summary>
    public Guid TimeSlotId { get; set; }

    /// <summary>
    /// Sequential order of this slot (1-18) within business hours
    /// </summary>
    public int SequenceOrder { get; set; }

    /// <summary>
    /// Start time of the slot (e.g., 08:00)
    /// </summary>
    public TimeOnly SlotStartTime { get; set; }

    /// <summary>
    /// End time of the slot (e.g., 08:30)
    /// </summary>
    public TimeOnly SlotEndTime { get; set; }

    /// <summary>
    /// The technician ID
    /// </summary>
    public Guid TechnicianId { get; set; }

    /// <summary>
    /// Technician's first name (for display)
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Technician's last name (for display)
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Dealership ID (for isolation)
    /// </summary>
    public Guid DealershipId { get; set; }

    /// <summary>
    /// Query date (for debugging/auditing)
    /// </summary>
    public DateOnly QueryDate { get; set; }

    /// <summary>
    /// Whether this slot is available for the technician
    /// (No conflicts with existing services or schedule)
    /// </summary>
    public bool IsAvailable { get; set; }
}
