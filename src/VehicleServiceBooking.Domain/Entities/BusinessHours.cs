namespace VehicleServiceBooking.Domain.Entities;

/// <summary>
/// BusinessHours represents the operating hours for a dealership on a specific day of the week
/// </summary>
public class BusinessHours : BaseEntity
{

    /// <summary>
    /// Reference to the dealership
    /// </summary>
    public Guid DealershipId { get; set; }

    /// <summary>
    /// Navigation property to the dealership
    /// </summary>
    public Dealership Dealership { get; set; } = null!;

    /// <summary>
    /// Day of the week (Monday, Tuesday, etc.)
    /// </summary>
    public DayOfWeek DayOfWeek { get; set; }

    /// <summary>
    /// Opening time on this day
    /// </summary>
    public TimeOnly OpenTime { get; set; }

    /// <summary>
    /// Closing time on this day
    /// </summary>
    public TimeOnly CloseTime { get; set; }
}