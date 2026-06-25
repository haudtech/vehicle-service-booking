using System.ComponentModel.DataAnnotations;

namespace VehicleServiceBooking.Domain.Entities;

/// <summary>
/// Dealership entity representing a vehicle service dealership
/// </summary>
public class Dealership : BaseEntity
{

    /// <summary>
    /// Dealership name (max 150 characters)
    /// </summary>
    [MaxLength(150)]
    [Required]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Dealership physical address (max 500 characters)
    /// </summary>
    [MaxLength(500)]
    [Required]
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Business hours for each day of the week
    /// </summary>
    public ICollection<BusinessHours> BusinessHours { get; set; }
        = new List<BusinessHours>();

    /// <summary>
    /// Collection of technicians working at this dealership
    /// </summary>
    public ICollection<Technician> Technicians { get; set; }
        = new List<Technician>();

    /// <summary>
    /// Collection of service bays at this dealership
    /// </summary>
    public ICollection<ServiceBay> ServiceBays { get; set; }
        = new List<ServiceBay>();

    /// <summary>
    /// Collection of appointments at this dealership
    /// </summary>
    public ICollection<Appointment> Appointments { get; set; }
        = new List<Appointment>();

    /// <summary>
    /// Collection of services provided at this dealership
    /// </summary>
    public ICollection<Service> Services { get; set; }
        = new List<Service>();
}