using System;

namespace VehicleServiceBooking.Application.DTOs;

/// <summary>
/// Response returned after updating an appointment status.
/// </summary>
public class AppointmentStatusUpdateResponse
{
    /// <summary>
    /// The appointment identifier that was updated.
    /// </summary>
    public Guid AppointmentId { get; set; }

    /// <summary>
    /// A human-readable message describing the completed action.
    /// </summary>
    public string Message { get; set; } = string.Empty;
}
