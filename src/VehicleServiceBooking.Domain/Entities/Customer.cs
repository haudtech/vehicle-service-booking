using System.ComponentModel.DataAnnotations;

namespace VehicleServiceBooking.Domain.Entities;

/// <summary>
/// Customer entity representing a customer who books vehicle services
/// </summary>
public class Customer : BaseEntity
{
    /// <summary>
    /// Customer's first name (max 100 characters)
    /// </summary>
    [MaxLength(100)]
    [Required]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Customer's last name (max 100 characters)
    /// </summary>
    [MaxLength(100)]
    [Required]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Customer's email address (max 254 characters per RFC 5321)
    /// </summary>
    [MaxLength(254)]
    [EmailAddress]
    [Required]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Customer's phone number in international format (max 20 characters)
    /// Example: +1-234-567-8900
    /// </summary>
    [MaxLength(20)]
    [Phone]
    [Required]
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Collection of vehicles owned by this customer
    /// </summary>
    public ICollection<Vehicle> Vehicles { get; set; }
        = new List<Vehicle>();

    /// <summary>
    /// Collection of appointments booked by this customer
    /// </summary>
    public ICollection<Appointment> Appointments { get; set; }
        = new List<Appointment>();
}