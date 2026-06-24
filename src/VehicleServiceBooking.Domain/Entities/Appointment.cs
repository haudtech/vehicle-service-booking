using VehicleServiceBooking.Domain.Enums;

namespace VehicleServiceBooking.Domain.Entities;

public class Appointment
{
    public Guid Id { get; set; }

    public Guid DealershipId { get; set; }

    public Guid CustomerId { get; set; }

    public Guid VehicleId { get; set; }

    public Guid ServiceTypeId { get; set; }

    public Guid TechnicianId { get; set; }

    public Guid ServiceBayId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public AppointmentStatus Status { get; set; } = AppointmentStatus.Booked;
}