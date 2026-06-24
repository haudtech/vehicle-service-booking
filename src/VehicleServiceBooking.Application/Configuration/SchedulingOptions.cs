namespace VehicleServiceBooking.Application.Configuration;

public class SchedulingOptions : ISchedulingConfiguration
{
    public const string SectionName = "Scheduling";

    public int SlotLengthMinutes { get; set; } = 30;
}