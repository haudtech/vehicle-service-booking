using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VehicleServiceBooking.Application.Interfaces.Persistence;
using VehicleServiceBooking.Application.Interfaces.Repositories;
using VehicleServiceBooking.Domain.Entities;

namespace VehicleServiceBooking.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Appointment entity persistence operations
/// Inherits from GenericRepository to leverage common query patterns with AsNoTracking()
/// </summary>
public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
{
    public AppointmentRepository(IApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public override async Task<Appointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await GetQueryable()
            .Include(a => a.Services)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Appointment>> GetByServiceBayAsync(
        Guid serviceBayId,
        DateTime startTime,
        DateTime endTime,
        CancellationToken cancellationToken)
    {
        // Get appointments that have services assigned to the specified service bay
        // Filter by service timing (EstimatedStartTime/EstimatedEndTime)
        return await GetQueryable()
            .Include(a => a.Services)
            .Where(a => a.Services.Any(s => 
                s.ServiceBayId == serviceBayId &&
                s.EstimatedStartTime >= startTime &&
                s.EstimatedEndTime <= endTime))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Appointment>> GetByDealershipAsync(
        Guid dealershipId,
        CancellationToken cancellationToken)
    {
        return await GetQueryable()
            .Where(a => a.DealershipId == dealershipId)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets all appointments for a specific vehicle with services included.
    /// Used for conflict detection and vehicle history queries.
    /// </summary>
    public async Task<IEnumerable<Appointment>> GetByVehicleIdAsync(
        Guid vehicleId,
        CancellationToken cancellationToken)
    {
        return await GetQueryable()
            .Where(a => a.VehicleId == vehicleId)
            .Include(a => a.Services)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Check if a service bay is available for a specific time slot on a given date
    /// Business Logic: NONE - Pure data query from view
    /// </summary>
    public async Task<bool> IsBayAvailableForSlotAsync(
        Guid serviceBayId,
        int startSequenceOrder,
        int endSequenceOrder,
        DateOnly appointmentDate,
        CancellationToken cancellationToken)
    {
        // Query the ServiceBayAvailableSlots view to check for any available slots
        // in the requested range. If any slot exists for this bay on this date,
        // the bay is available (view already filters out conflicts).
        var availableSlots = await DbContext.ServiceBayAvailableSlotsView
            .AsNoTracking()
            .Where(x =>
                x.ServiceBayId == serviceBayId &&
                x.QueryDate == appointmentDate &&
                x.SequenceOrder >= startSequenceOrder &&
                x.SequenceOrder <= endSequenceOrder &&
                x.IsAvailable)
            .ToListAsync(cancellationToken);

        // Bay is available if we have slots for the entire duration
        // (endSequenceOrder - startSequenceOrder + 1 slots needed)
        int requiredSlots = (endSequenceOrder - startSequenceOrder) + 1;
        return availableSlots.Count >= requiredSlots;
    }

    /// <summary>
    /// Verify that a technician has the required skill for a specific service type
    /// Business Logic: NONE - Pure data query
    /// </summary>
    public async Task<bool> TechnicianHasSkillAsync(
        Guid technicianId,
        Guid serviceTypeId,
        CancellationToken cancellationToken)
    {
        return await DbContext.TechnicianSkills
            .AsNoTracking()
            .AnyAsync(ts =>
                ts.TechnicianId == technicianId &&
                ts.ServiceTypeId == serviceTypeId,
                cancellationToken);
    }

    /// <summary>
    /// Create an appointment atomically with all its services in a single transaction
    /// Business Logic: NONE - Pure persistence
    /// </summary>
    public async Task<Appointment> CreateAppointmentWithServicesAsync(
        Appointment appointment,
        CancellationToken cancellationToken)
    {
        // Add the appointment entity
        // EF Core will cascade add all services in the Services collection
        DbContext.Appointments.Add(appointment);
        
        // Save everything atomically
        await SaveChangesAsync(cancellationToken);
        
        return appointment;
    }

    /// <summary>
    /// Get all TimeSlot IDs for a duration-based range
    /// Business Logic: NONE - Pure data query
    /// </summary>
    public async Task<IEnumerable<TimeSlot>> GetTimeSlotsBySequenceRangeAsync(
        int startSequenceOrder,
        int endSequenceOrder,
        CancellationToken cancellationToken)
    {
        return await DbContext.TimeSlots
            .AsNoTracking()
            .Where(t =>
                t.SequenceOrder >= startSequenceOrder &&
                t.SequenceOrder <= endSequenceOrder)
            .OrderBy(t => t.SequenceOrder)
            .ToListAsync(cancellationToken);
    }
}
