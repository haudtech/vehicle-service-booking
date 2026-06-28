using System;

namespace VehicleServiceBooking.Application.DTOs;

/// <summary>
/// Request to create a new appointment booking
/// </summary>
/// <remarks>
/// This DTO represents a complete request to book a vehicle service appointment.
/// All fields are required and must reference existing entities in the database.
/// 
/// The appointment booking system will:
/// 1. Validate that all GUIDs reference existing entities
/// 2. Check that the requested time slots are available
/// 3. Ensure no conflicting appointments exist at the same time/location
/// 4. Create the appointment record with booked status
/// 
/// Validation Rules:
/// - All GUID fields must be non-empty
/// - EstimatedStartTimeSlotId must reference a valid TimeSlot
/// - EstimatedEndTimeSlotId must reference a valid TimeSlot
/// - EndSlot must be after StartSlot (by SequenceOrder)
/// - Both time slots must be active (IsActive = true)
/// 
/// Note: The time slot IDs should be obtained from the /api/v1/availability endpoint
/// to ensure guaranteed availability at booking time.
/// 
/// TIMING MODEL:
/// - Client specifies StartTimeSlotId and EndTimeSlotId (grid-based, 30-min slots)
/// - Server stores these as FKs to the static TimeSlot table
/// - Combined with AppointmentDate, these determine the exact DateTime
/// </remarks>
public class CreateAppointmentRequest
{
    /// <summary>
    /// Dealership ID (where appointment is booked)
    /// </summary>
    /// <remarks>Must reference an existing dealership in the system</remarks>
    public Guid DealershipId { get; set; }

    /// <summary>
    /// Customer ID (who is booking)
    /// </summary>
    /// <remarks>Must reference an existing customer in the system</remarks>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Vehicle ID (what needs service)
    /// </summary>
    /// <remarks>Must reference an existing vehicle in the system and belong to the customer</remarks>
    public Guid VehicleId { get; set; }

    /// <summary>
    /// Appointment date (only the date portion is used)
    /// </summary>
    /// <remarks>
    /// The date when the appointment is scheduled.
    /// Combined with EstimatedStartTimeSlotId to determine the exact start time.
    /// Format: ISO 8601 (e.g., "2026-06-25")
    /// </remarks>
    public DateOnly AppointmentDate { get; set; }

    /// <summary>
    /// Service type ID (what service needed)
    /// </summary>
    /// <remarks>
    /// Must reference an existing service type in the system.
    /// The service type determines the duration and complexity of the appointment.
    /// </remarks>
    public Guid ServiceTypeId { get; set; }

    /// <summary>
    /// Technician ID (who will perform service)
    /// </summary>
    /// <remarks>
    /// Must reference an existing technician in the system.
    /// The technician should have the required skills for the service type.
    /// </remarks>
    public Guid TechnicianId { get; set; }

    /// <summary>
    /// Service bay ID (where service happens)
    /// </summary>
    /// <remarks>
    /// Must reference an existing service bay in the system.
    /// The service bay must be available for the entire requested time slot range.
    /// </remarks>
    public Guid ServiceBayId { get; set; }

    /// <summary>
    /// Estimated start time slot ID (FK to TimeSlot)
    /// </summary>
    /// <remarks>
    /// Must reference an existing TimeSlot (static reference data, 30-min grid).
    /// The TimeSlot.SequenceOrder indicates the position in the day (1-18 for 08:00-17:00).
    /// Combined with AppointmentDate, determines the exact start DateTime.
    /// </remarks>
    public Guid EstimatedStartTimeSlotId { get; set; }

    /// <summary>
    /// Estimated end time slot ID (FK to TimeSlot)
    /// </summary>
    /// <remarks>
    /// Must reference an existing TimeSlot (static reference data, 30-min grid).
    /// Must be after EstimatedStartTimeSlotId (by SequenceOrder).
    /// Combined with AppointmentDate, determines the exact end DateTime.
    /// </remarks>
    public Guid EstimatedEndTimeSlotId { get; set; }
}
