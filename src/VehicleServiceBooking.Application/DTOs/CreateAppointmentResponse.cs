using System;

namespace VehicleServiceBooking.Application.DTOs;

/// <summary>
/// Response after successfully creating an appointment
/// </summary>
/// <remarks>
/// This DTO is returned when an appointment is successfully created (HTTP 201).
/// It contains the appointment ID that can be used for future retrieval or reference.
/// 
/// The appointment ID should be stored by the client for:
/// - Retrieving appointment details via GET /api/v1/appointments/{id}
/// - Confirmation and receipt generation
/// - Customer communication and reminders
/// - Modification or cancellation requests (if implemented)
/// </remarks>
public class CreateAppointmentResponse
{
    /// <summary>
    /// Unique ID of the newly created appointment
    /// </summary>
    /// <remarks>
    /// This is a UUID v4 that uniquely identifies the appointment.
    /// Use this ID for all future references to this appointment.
    /// </remarks>
    public Guid AppointmentId { get; set; }

    /// <summary>
    /// Appointment start time (UTC)
    /// </summary>
    /// <remarks>
    /// The exact time when the service appointment begins.
    /// Format: ISO 8601 (e.g., "2026-06-25T10:00:00Z")
    /// </remarks>
    public DateTime SlotStart { get; set; }

    /// <summary>
    /// Appointment end time (UTC)
    /// </summary>
    /// <remarks>
    /// The exact time when the service appointment ends.
    /// The duration is SlotEnd - SlotStart.
    /// Format: ISO 8601 (e.g., "2026-06-25T11:00:00Z")
    /// </remarks>
    public DateTime SlotEnd { get; set; }

    /// <summary>
    /// When the appointment was created in the system
    /// </summary>
    /// <remarks>
    /// Server timestamp indicating when the booking was confirmed.
    /// This is used for audit trails and receipt generation.
    /// Format: ISO 8601 (e.g., "2026-06-25T09:30:00Z")
    /// </remarks>
    public DateTime CreatedAt { get; set; }
}
