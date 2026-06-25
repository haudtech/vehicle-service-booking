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
/// 2. Check that the requested time slot is available for the specified service bay
/// 3. Ensure no conflicting appointments exist at the same time/location
/// 4. Create the appointment record with booked status
/// 
/// Validation Rules:
/// - All GUID fields must be non-empty
/// - SlotStart must be in UTC and in the future
/// - SlotEnd must be after SlotStart
/// - SlotStart and SlotEnd should be within business hours (per dealership configuration)
/// 
/// Note: The time slot should be obtained from the /api/v1/availability endpoint
/// to ensure guaranteed availability at booking time.
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
    /// The service bay must be available for the entire requested time slot.
    /// </remarks>
    public Guid ServiceBayId { get; set; }

    /// <summary>
    /// Appointment start time (UTC)
    /// </summary>
    /// <remarks>
    /// Must be a valid UTC datetime in the future.
    /// Should align with dealership business hours.
    /// Format: ISO 8601 (e.g., "2026-06-25T10:00:00Z")
    /// </remarks>
    public DateTime SlotStart { get; set; }

    /// <summary>
    /// Appointment end time (UTC)
    /// </summary>
    /// <remarks>
    /// Must be after SlotStart.
    /// The duration determines how long the service bay will be occupied.
    /// Format: ISO 8601 (e.g., "2026-06-25T11:00:00Z")
    /// </remarks>
    public DateTime SlotEnd { get; set; }
}

