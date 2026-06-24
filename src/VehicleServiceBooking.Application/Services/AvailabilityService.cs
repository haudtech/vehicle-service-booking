using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VehicleServiceBooking.Application.Interfaces.Persistence;
using VehicleServiceBooking.Application.Configuration;
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

    public async Task<List<TimeSlot>> GetAvailableSlotsAsync(
        Guid dealershipId,
        Guid serviceTypeId,
        DateTime date,
        CancellationToken cancellationToken = default)
    {
        int durationMinutes =
            await GetServiceDurationAsync(serviceTypeId, cancellationToken);

        int requiredSlots =
            CalculateRequiredSlots(durationMinutes);

        var (start, end) =
            GetWorkingHoursWindow(date);

        var slots = GenerateTimeSlots(date, start, end);

        var candidateWindows =
            GenerateCandidateBookingWindows(slots, requiredSlots);

        var appointments =
            await _dbContext.Appointments
                .Where(a =>
                    a.DealershipId == dealershipId &&
                    a.StartTime.Date == date.Date)
                .ToListAsync(cancellationToken);

        var availableWindows =
            candidateWindows
                .Where(window => !IsConflicting(window, appointments))
                .ToList();

        return availableWindows;
    }

    private async Task<int> GetServiceDurationAsync(
        Guid serviceTypeId,
        CancellationToken cancellationToken)
    {
        var serviceType =
            await _dbContext.ServiceTypes
                .FirstAsync(x => x.Id == serviceTypeId, cancellationToken);

        return serviceType.DurationMinutes;
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

    private bool IsConflicting(
        TimeSlot window,
        List<Domain.Entities.Appointment> appointments)
    {
        return appointments.Any(a =>
            Overlaps(window.Start, window.End, a.StartTime, a.EndTime));
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