using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VehicleServiceBooking.Application.Configuration.Interfaces;
using VehicleServiceBooking.Application.Interfaces;
using VehicleServiceBooking.Application.Interfaces.Persistence;
using VehicleServiceBooking.Application.Interfaces.Repositories;
using VehicleServiceBooking.Application.Interfaces.Services;
using VehicleServiceBooking.Application.Models;

namespace VehicleServiceBooking.Application.Services;

public class AvailabilityService : IAvailabilityService
{
    private readonly ISchedulingConfiguration _schedulingConfiguration;
    private readonly IApplicationDbContext _dbContext;
    private readonly IAvailabilityRepository _availabilityRepository;

    public AvailabilityService(
        ISchedulingConfiguration schedulingConfiguration,
        IApplicationDbContext dbContext,
        IAvailabilityRepository availabilityRepository)
    {
        _schedulingConfiguration = schedulingConfiguration;
        _dbContext = dbContext;
        _availabilityRepository = availabilityRepository;
    }

    public async Task<List<AvailabilityOption>> GetAvailableSlotsAsync(
        Guid dealershipId,
        Guid serviceTypeId,
        DateTime date,
        CancellationToken cancellationToken = default)
    {
        // -----------------------------
        // 1. Service duration
        // -----------------------------
        var serviceType =
            await _dbContext.ServiceTypes
                .FirstAsync(x => x.Id == serviceTypeId, cancellationToken);

        int durationMinutes = serviceType.DurationMinutes;

        int requiredSlots =
            CalculateRequiredSlots(durationMinutes);

        // -----------------------------
        // 2. Load technicians
        // -----------------------------
        var technicians =
            await _dbContext.Technicians
                .Where(t => t.DealershipId == dealershipId)
                .ToListAsync(cancellationToken);

        // -----------------------------
        // 3. Load technician skills
        // -----------------------------
        var technicianSkills =
            await _dbContext.TechnicianSkills
                .Where(ts => ts.ServiceTypeId == serviceTypeId)
                .ToListAsync(cancellationToken);

        var eligibleTechnicians =
            GetEligibleTechnicians(technicianSkills);

        // -----------------------------
        // 4. Load schedules
        // -----------------------------
        var schedules =
            await _dbContext.TechnicianSchedules
                .Where(s =>
                    technicians.Select(t => t.Id)
                        .Contains(s.TechnicianId) &&
                    s.DayOfWeek == date.DayOfWeek)
                .ToListAsync(cancellationToken);

        // -----------------------------
        // 5. Load appointments
        // -----------------------------
        var appointments =
            await _dbContext.Appointments
                .Where(a =>
                    a.DealershipId == dealershipId &&
                    a.StartTime.Date == date.Date &&
                    eligibleTechnicians.Contains(a.TechnicianId))
                .ToListAsync(cancellationToken);

        var bayAppointments =
            await _dbContext.Appointments
                .Where(a =>
                    a.DealershipId == dealershipId &&
                    a.StartTime.Date == date.Date)
                .ToListAsync(cancellationToken);

        // -----------------------------
        // 6. Load service bays
        // -----------------------------
        var serviceBays =
            await _dbContext.ServiceBays
                .Where(b => b.DealershipId == dealershipId)
                .ToListAsync(cancellationToken);

        // -----------------------------
        // 7. Generate slots
        // -----------------------------
        var (start, end) = GetWorkingHoursWindow(date);

        var slots = GenerateTimeSlots(date, start, end);

        var candidateWindows =
            GenerateCandidateBookingWindows(slots, requiredSlots);

        // -----------------------------
        // 8. Build final allocation results
        // -----------------------------
        var results = new List<AvailabilityOption>();

        foreach (var window in candidateWindows)
        {
            foreach (var techId in eligibleTechnicians)
            {
                if (!IsTechnicianAvailable(techId, window, appointments))
                    continue;

                if (!IsWithinTechnicianSchedule(techId, window, schedules))
                    continue;

                var bay = serviceBays.FirstOrDefault(b =>
                    IsBayAvailable(b.Id, window, bayAppointments));

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

    // -----------------------------
    // Helpers
    // -----------------------------
    private HashSet<Guid> GetEligibleTechnicians(
        List<Domain.Entities.TechnicianSkill> skills)
    {
        return skills
            .Select(s => s.TechnicianId)
            .ToHashSet();
    }

    private bool IsTechnicianAvailable(
        Guid technicianId,
        TimeSlot window,
        List<Domain.Entities.Appointment> appointments)
    {
        return !appointments.Any(a =>
            a.TechnicianId == technicianId &&
            Overlaps(window.Start, window.End, a.StartTime, a.EndTime));
    }

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

    private bool IsBayAvailable(
        Guid bayId,
        TimeSlot window,
        List<Domain.Entities.Appointment> appointments)
    {
        return !appointments.Any(a =>
            a.ServiceBayId == bayId &&
            Overlaps(window.Start, window.End, a.StartTime, a.EndTime));
    }

    private int CalculateRequiredSlots(int durationMinutes)
    {
        return (int)Math.Ceiling(
            durationMinutes /
            (double)_schedulingConfiguration.SlotLengthMinutes);
    }

    private (TimeSpan Start, TimeSpan End) GetWorkingHoursWindow(DateTime date)
    {
        return (
            new TimeSpan(8, 0, 0),
            new TimeSpan(17, 0, 0)
        );
    }

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

    private bool Overlaps(
        DateTime startA,
        DateTime endA,
        DateTime startB,
        DateTime endB)
    {
        return startA < endB && startB < endA;
    }
}