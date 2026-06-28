namespace VehicleServiceBooking.Domain.Entities;

/// <summary>
/// Vehicle entity representing a vehicle that receives service
/// Configuration for properties is in ApplicationDbContext.OnModelCreating()
/// </summary>
public class Vehicle : BaseEntity
{
    /// <summary>
    /// Reference to the customer who owns this vehicle
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Vehicle Identification Number (always 17 characters)
    /// </summary>
    public string Vin { get; set; } = string.Empty;

    /// <summary>
    /// Vehicle manufacturer (max 20 characters)
    /// Example: Toyota, Ford, Honda
    /// </summary>
    public string Make { get; set; } = string.Empty;

    /// <summary>
    /// Vehicle model name (max 50 characters)
    /// Example: Camry, Mustang, CR-V
    /// </summary>
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// Vehicle model year (typically 4 digits)
    /// OPTIONAL: Can be null if client doesn't provide the information
    /// </summary>
    public int? Year { get; set; }

    /// <summary>
    /// Navigation property to the customer who owns this vehicle
    /// </summary>
    public Customer Customer { get; set; } = null!;

    /// <summary>
    /// Collection of appointments for this vehicle
    /// </summary>
    public ICollection<Appointment> Appointments { get; set; }
        = new List<Appointment>();
}