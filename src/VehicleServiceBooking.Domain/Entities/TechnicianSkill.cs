namespace VehicleServiceBooking.Domain.Entities;

public class TechnicianSkill
{
    public Guid Id { get; set; }

    public Guid TechnicianId { get; set; }

    public Guid ServiceTypeId { get; set; }
}