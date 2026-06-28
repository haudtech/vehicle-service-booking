using System;

namespace VehicleServiceBooking.Application.Models;

public class AvailabilityOption
{
    public DateTimeSlot DateTimeSlot { get; set; } = default!;

    public Guid TechnicianId { get; set; }

    public Guid ServiceBayId { get; set; }
}