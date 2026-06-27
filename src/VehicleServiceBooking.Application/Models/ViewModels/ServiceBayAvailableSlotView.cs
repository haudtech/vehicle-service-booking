using System;

namespace VehicleServiceBooking.Application.Models.ViewModels;

/// <summary>
/// Represents a time slot available for a specific service bay on a given date.
/// This DTO is populated from the ServiceBayAvailableSlots SQL view.
/// 
/// Used by: AvailabilityService to determine which service bays can accommodate services
/// in which time slots without conflicts.
/// </summary>
public class ServiceBayAvailableSlotsView
{
    /// <summary>
    /// Reference to the TimeSlot entity (Part of composite key)
    /// </summary>
    public Guid TimeSlotId { get; set; }

    /// <summary>
    /// The service bay ID (Part of composite key)
    /// </summary>
    public Guid ServiceBayId { get; set; }

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
    /// Whether this slot is available for the bay
    /// (No conflicts with existing services, bay is active)
    /// </summary>
    public bool IsAvailable { get; set; }
}
