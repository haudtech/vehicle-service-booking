namespace VehicleServiceBooking.Domain.Entities;

public class Technician
{
    public Guid Id { get; set; }

    public Guid DealershipId { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public ICollection<TechnicianSchedule> Schedules { get; set; }
        = new List<TechnicianSchedule>();

    public ICollection<TechnicianSkill> Skills { get; set; } 
        = new List<TechnicianSkill>();
}