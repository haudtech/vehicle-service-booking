namespace VehicleServiceBooking.Domain.Entities;

public class Dealership
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public ICollection<BusinessHours> BusinessHours { get; set; }
        = new List<BusinessHours>();
}