using System;
using System.Collections.Generic;

namespace VehicleServiceBooking.Domain.Entities;

/// <summary>
/// TimeSlot represents a fixed 30-minute time window in the dealership's business day.
/// Static reference data: 36 slots from 08:00 to 17:00.
/// All Service entities reference these slots for scheduling.
/// 
/// This is an immutable reference table that serves as the foundation for the grid-based
/// scheduling system. Instead of calculating slots at runtime, we maintain this fixed set
/// of 36 daily slots, allowing for O(1) lookups and simplified conflict detection.
/// </summary>
public class TimeSlot : BaseEntity
{
    /// <summary>
    /// Sequential order of this slot in the day (1-36).
    /// Examples:
    /// - 1: 08:00-08:30
    /// - 2: 08:30-09:00
    /// - ...
    /// - 36: 16:30-17:00
    /// </summary>
    public int SequenceOrder { get; set; }

    /// <summary>
    /// Start time within a business day (e.g., 08:00).
    /// Uses TimeOnly type - no date component needed since this is a template for every day.
    /// Combined with Appointment.AppointmentDate by consuming code to get full DateTime.
    /// </summary>
    public TimeOnly SlotStartTime { get; set; }

    /// <summary>
    /// End time within a business day (e.g., 08:30).
    /// Combined with Appointment.AppointmentDate for full DateTime.
    /// Always SlotStartTime + 30 minutes.
    /// </summary>
    public TimeOnly SlotEndTime { get; set; }

    /// <summary>
    /// Whether this slot is currently active (for soft-delete).
    /// Inactive slots can be skipped during availability queries.
    /// Allows dealerships to close certain slots without deleting the reference data.
    /// </summary>
    public bool IsActive { get; set; } = true;

    // ============================================
    // Navigation Properties
    // ============================================

    /// <summary>
    /// Services that ESTIMATE to START in this slot.
    /// Foreign key: Service.EstimatedStartTimeSlotId
    /// 
    /// This allows efficient lookup of "which services start in slot X?"
    /// Used by conflict detection algorithms.
    /// </summary>
    public ICollection<Service> EstimatedStartServices { get; set; } = [];

    /// <summary>
    /// Services that ESTIMATE to END in this slot.
    /// Foreign key: Service.EstimatedEndTimeSlotId
    /// 
    /// This allows efficient lookup of "which services end in slot X?"
    /// Used by conflict detection algorithms.
    /// </summary>
    public ICollection<Service> EstimatedEndServices { get; set; } = [];
}
