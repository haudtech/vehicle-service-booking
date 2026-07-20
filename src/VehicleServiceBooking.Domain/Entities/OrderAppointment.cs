namespace VehicleServiceBooking.Domain.Entities;

/// <summary>
/// Junction table between orders and appointments.
/// Supports one order to many appointments while enforcing each appointment in at most one order.
/// </summary>
public class OrderAppointment : BaseEntity
{
    public Guid OrderId { get; set; }

    public Guid AppointmentId { get; set; }

    public Order Order { get; set; } = null!;

    public Appointment Appointment { get; set; } = null!;
}
