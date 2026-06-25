namespace VehicleServiceBooking.Application.Configuration.Interfaces;

/// <summary>
/// Configuration for scheduling and appointment slot management
/// </summary>
public interface ISchedulingConfiguration
{
    /// <summary>
    /// Duration of each appointment slot in minutes
    /// </summary>
    int SlotLengthMinutes { get; }
}
