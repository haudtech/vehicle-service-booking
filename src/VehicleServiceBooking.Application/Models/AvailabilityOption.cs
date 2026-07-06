using System;

namespace VehicleServiceBooking.Application.Models;

public class AvailabilityOption
{
    public Guid TimeSlotId { get; set; }

    public Guid EndTimeSlotId { get; set; }

    public DateTimeSlot DateTimeSlot { get; set; } = default!;

    public Guid TechnicianId { get; set; }

    public Guid ServiceBayId { get; set; }
}