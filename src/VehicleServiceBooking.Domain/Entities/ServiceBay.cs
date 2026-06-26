namespace VehicleServiceBooking.Domain.Entities;

/// <summary>
/// ServiceBay entity representing a service bay at a dealership
/// where vehicle services are performed
/// Configuration for properties is in ApplicationDbContext.OnModelCreating()
/// </summary>
public class ServiceBay : BaseEntity
{

    /// <summary>
    /// Reference to the dealership that owns this service bay
    /// </summary>
    public Guid DealershipId { get; set; }

    /// <summary>
    /// Navigation property to the dealership
    /// </summary>
    public Dealership Dealership { get; set; } = null!;

    /// <summary>
    /// Service bay name/identifier (max 50 characters)
    /// Examples: Bay A, Bay 1, Lift 1
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Indicates if the service bay is available for use
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Collection of services performed in this service bay
    /// </summary>
    public ICollection<Service> Services { get; set; }
        = new List<Service>();
}