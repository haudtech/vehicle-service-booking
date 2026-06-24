namespace VehicleServiceBooking.Domain.Entities;

public class Customer
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public ICollection<Vehicle> Vehicles { get; set; }
        = new List<Vehicle>();
}