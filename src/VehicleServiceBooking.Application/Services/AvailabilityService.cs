using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VehicleServiceBooking.Application.Configuration.Interfaces;
using VehicleServiceBooking.Application.Interfaces.Persistence;
using VehicleServiceBooking.Application.Interfaces.Services;
using VehicleServiceBooking.Application.Models;

namespace VehicleServiceBooking.Application.Services;

public class AvailabilityService : IAvailabilityService
{
    private readonly ISchedulingConfiguration _schedulingConfiguration;
    private readonly IApplicationDbContext _dbContext;

    public AvailabilityService(
        ISchedulingConfiguration schedulingConfiguration,
        IApplicationDbContext dbContext)
    {
        _schedulingConfiguration = schedulingConfiguration;
        _dbContext = dbContext;
    }

    public async Task<List<AvailabilityOption>> GetAvailableSlotsAsync(
        Guid dealershipId,
        Guid serviceTypeId,
        DateTime date,
        CancellationToken cancellationToken = default)
    {
        // Get service type duration
        var serviceType =
            await _dbContext.ServiceTypes
                .FirstAsync(x => x.Id == serviceTypeId, cancellationToken);

        int durationMinutes = serviceType.DurationMinutes;
        int requiredSlots = CalculateRequiredSlots(durationMinutes);

        // Load technicians for the dealership
        var technicians =
            await _dbContext.Technicians
                .Where(t => t.DealershipId == dealershipId)
                .ToListAsync(cancellationToken);

        // Load technician skills for the service type
        var technicianSkills =
            await _dbContext.TechnicianSkills
                .Where(ts => ts.ServiceTypeId == serviceTypeId)
                .ToListAsync(cancellationToken);

        var eligibleTechnicians = GetEligibleTechnicians(technicianSkills);

        // Load technician schedules for the date
        var schedules =
            await _dbContext.TechnicianSchedules
                .Where(s =>
                    technicians.Select(t => t.Id)
                        .Contains(s.TechnicianId) &&
                    s.DayOfWeek == date.DayOfWeek)
                .ToListAsync(cancellationToken);

        // Load services with technician assignments for conflict checking
        // Filter by appointment date, not service start time
        var servicesWithTechs =
            await _dbContext.Services
                .Where(s =>
                    s.DealershipId == dealershipId &&
                    s.TechnicianId != null &&
                    s.Appointment.AppointmentDate == DateOnly.FromDateTime(date))
                .ToListAsync(cancellationToken);

        // Load services for bay conflict checking
        var bayServices =
            await _dbContext.Services
                .Where(s =>
                    s.DealershipId == dealershipId &&
                    s.Appointment.AppointmentDate == DateOnly.FromDateTime(date))
                .ToListAsync(cancellationToken);

        // Load service bays
        var serviceBays =
            await _dbContext.ServiceBays
                .Where(b => b.DealershipId == dealershipId)
                .ToListAsync(cancellationToken);

        // Generate time slots for the working day
        var (start, end) = GetWorkingHoursWindow(date);
        var slots = GenerateTimeSlots(date, start, end);
        var candidateWindows = GenerateCandidateBookingWindows(slots, requiredSlots);

        // Build availability options
        var results = new List<AvailabilityOption>();

        foreach (var window in candidateWindows)
        {
            foreach (var techId in eligibleTechnicians)
            {
                // Check technician availability
                if (!IsTechnicianAvailable(techId, window, servicesWithTechs))
                    continue;

                // Check technician schedule
                if (!IsWithinTechnicianSchedule(techId, window, schedules))
                    continue;

                // Find available bay
                var bay = serviceBays.FirstOrDefault(b =>
                    IsBayAvailable(b.Id, window, bayServices));

                if (bay == null)
                    continue;

                results.Add(new AvailabilityOption
                {
                    TimeSlot = window,
                    TechnicianId = techId,
                    ServiceBayId = bay.Id
                });
            }
        }

        return results;
    }

    /// <summary>
    /// Gets eligible technicians who have skills for the given service type
    /// </summary>
    private HashSet<Guid> GetEligibleTechnicians(
        List<Domain.Entities.TechnicianSkill> skills)
    {
        return skills
            .Select(s => s.TechnicianId)
            .ToHashSet();
    }

    /// <summary>
    /// Checks if a technician is available for the given time window
    /// </summary>
    private bool IsTechnicianAvailable(
        Guid technicianId,
        TimeSlot window,
        List<Domain.Entities.Service> services)
    {
        return !services.Any(s =>
            s.TechnicianId == technicianId &&
            s.EstimatedStartTime.HasValue &&
            s.EstimatedEndTime.HasValue &&
            Overlaps(window.Start, window.End, s.EstimatedStartTime.Value, s.EstimatedEndTime.Value));
    }

    /// <summary>
    /// Checks if the time window is within technician's schedule
    /// </summary>
    private bool IsWithinTechnicianSchedule(
        Guid technicianId,
        TimeSlot window,
        List<Domain.Entities.TechnicianSchedule> schedules)
    {
        var start = TimeOnly.FromTimeSpan(window.Start.TimeOfDay);
        var end = TimeOnly.FromTimeSpan(window.End.TimeOfDay);

        return schedules.Any(s =>
            s.TechnicianId == technicianId &&
            start >= s.StartTime &&
            end <= s.EndTime);
    }

    /// <summary>
    /// Checks if a service bay is available for the given time window
    /// </summary>
    private bool IsBayAvailable(
        Guid bayId,
        TimeSlot window,
        List<Domain.Entities.Service> services)
    {
        return !services.Any(s =>
            s.ServiceBayId == bayId &&
            s.EstimatedStartTime.HasValue &&
            s.EstimatedEndTime.HasValue &&
            Overlaps(window.Start, window.End, s.EstimatedStartTime.Value, s.EstimatedEndTime.Value));
    }

    /// <summary>
    /// Calculates the number of time slots required for the service duration
    /// </summary>
    private int CalculateRequiredSlots(int durationMinutes)
    {
        return (int)Math.Ceiling(
            durationMinutes /
            (double)_schedulingConfiguration.SlotLengthMinutes);
    }

    /// <summary>
    /// Gets the working hours window for the dealership
    /// </summary>
    private (TimeSpan Start, TimeSpan End) GetWorkingHoursWindow(DateTime date)
    {
        // Default working hours: 8 AM to 5 PM
        return (new TimeSpan(8, 0, 0), new TimeSpan(17, 0, 0));
    }

    /// <summary>
    /// Generates 15-minute time slots for the working day
    /// </summary>
    private List<TimeSlot> GenerateTimeSlots(
        DateTime date,
        TimeSpan start,
        TimeSpan end)
    {
        var result = new List<TimeSlot>();
        var current = start;

        while (current.Add(TimeSpan.FromMinutes(_schedulingConfiguration.SlotLengthMinutes)) <= end)
        {
            var slotStart = date.Date.Add(current);
            var slotEnd = slotStart.AddMinutes(_schedulingConfiguration.SlotLengthMinutes);

            result.Add(new TimeSlot
            {
                Start = slotStart,
                End = slotEnd
            });

            current = current.Add(TimeSpan.FromMinutes(_schedulingConfiguration.SlotLengthMinutes));
        }

        return result;
    }

    /// <summary>
    /// Generates candidate booking windows from individual slots based on required duration
    /// </summary>
    private List<TimeSlot> GenerateCandidateBookingWindows(
        List<TimeSlot> slots,
        int requiredSlots)
    {
        var result = new List<TimeSlot>();

        for (int i = 0; i <= slots.Count - requiredSlots; i++)
        {
            var start = slots[i].Start;
            var end = slots[i + requiredSlots - 1].End;

            result.Add(new TimeSlot
            {
                Start = start,
                End = end
            });
        }

        return result;
    }

    /// <summary>
    /// Determines if two time ranges overlap
    /// </summary>
    private bool Overlaps(
        DateTime startA,
        DateTime endA,
        DateTime startB,
        DateTime endB)
    {
        return startA < endB && startB < endA;
    }
}
