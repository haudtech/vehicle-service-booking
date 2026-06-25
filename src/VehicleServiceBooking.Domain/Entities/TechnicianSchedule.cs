namespace VehicleServiceBooking.Domain.Entities;

/// <summary>
/// TechnicianSchedule represents work hours for a technician on a specific day
/// </summary>
public class TechnicianSchedule : BaseEntity
{

    /// <summary>
    /// Reference to the technician
    /// </summary>
    public Guid TechnicianId { get; set; }

    /// <summary>
    /// Navigation property to the technician
    /// </summary>
    public Technician Technician { get; set; } = null!;

    /// <summary>
    /// Day of the week (Monday, Tuesday, etc.)
    /// </summary>
    public DayOfWeek DayOfWeek { get; set; }

    /// <summary>
    /// Work start time on this day
    /// </summary>
    public TimeOnly StartTime { get; set; }

    /// <summary>
    /// Work end time on this day
    /// </summary>
    public TimeOnly EndTime { get; set; }
}