namespace VehicleServiceBooking.Application.Configuration;

public class SchedulingOptions : ISchedulingConfiguration
{
    public const string SectionName = "Scheduling";

    /// <summary>
    /// Duration of each appointment slot in minutes
    /// </summary>
    public int SlotLengthMinutes { get; set; } = 30;
}