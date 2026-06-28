using System;

namespace VehicleServiceBooking.Application.Models;

/// <summary>
/// Minimal projected shape returned by availability queries.
/// Keeps repository reads lean by selecting only fields needed by the service layer.
/// </summary>
public sealed class AvailabilityProjection
{
    /// <summary>
    /// Start slot identifier from availability view.
    /// </summary>
    public Guid TimeSlotId { get; set; }

    /// <summary>
    /// Slot start time.
    /// </summary>
    public TimeOnly SlotStartTime { get; set; }

    /// <summary>
    /// Slot end time.
    /// </summary>
    public TimeOnly SlotEndTime { get; set; }

    /// <summary>
    /// Assigned technician for this slot option.
    /// </summary>
    public Guid TechnicianId { get; set; }

    /// <summary>
    /// Assigned service bay for this slot option.
    /// </summary>
    public Guid ServiceBayId { get; set; }
}