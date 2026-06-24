namespace VehicleServiceBooking.Domain.Entities;

public class BusinessHours
{
    public Guid Id { get; set; }

    public Guid DealershipId { get; set; }

    public DayOfWeek DayOfWeek { get; set; }

    public TimeOnly OpenTime { get; set; }

    public TimeOnly CloseTime { get; set; }
}