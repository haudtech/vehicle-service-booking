using VehicleServiceBooking.Domain.Enums;

namespace VehicleServiceBooking.Domain.Entities;

/// <summary>
/// Appointment entity representing a vehicle service booking
/// </summary>
public class Appointment : BaseEntity
{

    /// <summary>
    /// Reference to the dealership where the appointment is scheduled
    /// </summary>
    public Guid DealershipId { get; set; }

    /// <summary>
    /// Navigation property to the dealership
    /// </summary>
    public Dealership Dealership { get; set; } = null!;

    /// <summary>
    /// Reference to the customer who booked the appointment
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Navigation property to the customer
    /// </summary>
    public Customer Customer { get; set; } = null!;

    /// <summary>
    /// Reference to the vehicle being serviced
    /// </summary>
    public Guid VehicleId { get; set; }

    /// <summary>
    /// Navigation property to the vehicle
    /// </summary>
    public Vehicle Vehicle { get; set; } = null!;

    /// <summary>
    /// The date of the appointment (single day only - appointment cannot span multiple days)
    /// Individual services within the appointment handle their own StartTime and EndTime
    /// </summary>
    public DateOnly AppointmentDate { get; set; }

    /// <summary>
    /// Reference to the appointment status (FK to AppointmentStatusLookup)
    /// </summary>
    public Guid StatusId { get; set; }

    /// <summary>
    /// Navigation property to the appointment status lookup
    /// </summary>
    public AppointmentStatusLookup Status { get; set; } = null!;

    /// <summary>
    /// General notes for the appointment
    /// </summary>
    public string Notes { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property to the services included in this appointment
    /// </summary>
    public ICollection<Service> Services { get; set; } = new List<Service>();
}