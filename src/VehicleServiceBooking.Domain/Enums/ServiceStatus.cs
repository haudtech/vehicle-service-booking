namespace VehicleServiceBooking.Domain.Enums;

/// <summary>
/// ServiceStatus enum representing the different statuses a service can have within an appointment
/// </summary>
public enum ServiceStatus
{
    /// <summary>
    /// Service is pending (scheduled but not yet started)
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Service is currently in progress
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// Service has been completed successfully
    /// </summary>
    Completed = 2,

    /// <summary>
    /// Service has been skipped (customer cancelled or declined)
    /// </summary>
    Skipped = 3,

    /// <summary>
    /// Service has been rescheduled to a different appointment
    /// </summary>
    Rescheduled = 4
}
