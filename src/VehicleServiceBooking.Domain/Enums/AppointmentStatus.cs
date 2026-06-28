namespace VehicleServiceBooking.Domain.Enums;

/// <summary>
/// AppointmentStatus enum representing the different statuses an appointment can have
/// </summary>
public enum AppointmentStatus
{
    /// <summary>
    /// Appointment is scheduled (booked)
    /// </summary>
    Booked = 1,

    /// <summary>
    /// Appointment is currently in progress
    /// </summary>
    InProgress = 2,

    /// <summary>
    /// Appointment has been completed (all services done)
    /// </summary>
    Completed = 3,

    /// <summary>
    /// Appointment has been cancelled
    /// </summary>
    Cancelled = 4,

    /// <summary>
    /// Appointment is partially completed (some services done, some rescheduled)
    /// </summary>
    PartiallyCompleted = 5
}