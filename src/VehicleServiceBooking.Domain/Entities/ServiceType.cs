namespace VehicleServiceBooking.Domain.Entities;

public class ServiceType
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int DurationMinutes { get; set; }

    public ICollection<TechnicianSkill> TechnicianSkills { get; set; }
        = new List<TechnicianSkill>();
}