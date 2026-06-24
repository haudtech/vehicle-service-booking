namespace VehicleServiceBooking.Domain.Entities;

public class TechnicianSchedule
{
    public Guid Id { get; set; }

    public Guid TechnicianId { get; set; }

    public DayOfWeek DayOfWeek { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }
}