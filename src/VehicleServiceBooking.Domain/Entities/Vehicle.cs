namespace VehicleServiceBooking.Domain.Entities;

public class Vehicle
{
    public Guid Id { get; set; }

    public Guid CustomerId { get; set; }

    public string Vin { get; set; } = string.Empty;

    public string Make { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;

    public int Year { get; set; }
}