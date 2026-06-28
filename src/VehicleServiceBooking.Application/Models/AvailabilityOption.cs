using System;

namespace VehicleServiceBooking.Application.Models;

public class AvailabilityOption
{
    public TimeSlot TimeSlot { get; set; } = default!;

    public Guid TechnicianId { get; set; }

    public Guid ServiceBayId { get; set; }
}