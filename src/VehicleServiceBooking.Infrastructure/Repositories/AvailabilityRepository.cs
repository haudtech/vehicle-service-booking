using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VehicleServiceBooking.Application.Interfaces.Persistence;
using VehicleServiceBooking.Application.Interfaces.Repositories;
using VehicleServiceBooking.Application.Models.ViewModels;
using VehicleServiceBooking.Domain.Entities;

namespace VehicleServiceBooking.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for availability checking and slot management
/// </summary>
public class AvailabilityRepository : IAvailabilityRepository
{
    private readonly IApplicationDbContext _dbContext;

    public AvailabilityRepository(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<IEnumerable<Appointment>> GetAppointmentsByServiceTypeAsync(
        Guid serviceTypeId,
        DateTime startTime,
        DateTime endTime,
        CancellationToken cancellationToken)
    {
        // Get appointments that have services of the specified service type
        // Filter by service timing (EstimatedStartTime/EstimatedEndTime)
        var appointments = await _dbContext.Appointments
            .AsNoTracking()
            .Where(a => a.Services.Any(s => 
                s.ServiceTypeId == serviceTypeId &&
                s.EstimatedStartTime >= startTime &&
                s.EstimatedEndTime <= endTime))
            .ToListAsync(cancellationToken);

        return appointments;
    }

    public async Task<IEnumerable<Technician>> GetTechniciansByDealershipAsync(
        Guid dealershipId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Technicians
            .AsNoTracking()
            .Where(t => t.DealershipId == dealershipId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Technician>> GetQualifiedTechniciansAsync(
        Guid serviceTypeId,
        Guid dealershipId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Technicians
            .AsNoTracking()
            .Where(t => t.DealershipId == dealershipId
                && t.Skills.Any(s => s.ServiceTypeId == serviceTypeId))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ServiceBay>> GetServiceBaysAsync(
        Guid dealershipId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.ServiceBays
            .AsNoTracking()
            .Where(s => s.DealershipId == dealershipId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Appointment>> GetAppointmentsInRangeAsync(
        DateTime startTime,
        DateTime endTime,
        CancellationToken cancellationToken)
    {
        // Get appointments that have services within the time range
        // Filter by service timing (EstimatedStartTime/EstimatedEndTime)
        return await _dbContext.Appointments
            .AsNoTracking()
            .Where(a => a.Services.Any(s =>
                s.EstimatedStartTime >= startTime && 
                s.EstimatedEndTime <= endTime))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<BusinessHours>> GetBusinessHoursAsync(
        Guid dealershipId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.BusinessHours
            .AsNoTracking()
            .Where(b => b.DealershipId == dealershipId)
            .ToListAsync(cancellationToken);
    }

    // ==================== PHASE 4: MATERIALIZED VIEW QUERY METHODS ====================

    /// <summary>
    /// Query ServiceTypeAvailability view for all service types and their available options
    /// Business Logic: NONE - Pure data query from materialized view
    /// Performance: Single database query (< 50ms)
    /// </summary>
    public async Task<IEnumerable<ServiceTypeAvailabilityView>> GetServiceTypeAvailabilityAsync(
        Guid dealershipId,
        Guid[] serviceTypeIds,
        DateOnly queryDate,
        CancellationToken cancellationToken)
    {
        return await _dbContext.ServiceTypeAvailabilityView
            .AsNoTracking()
            .Where(x =>
                x.DealershipId == dealershipId &&
                x.QueryDate == queryDate &&
                serviceTypeIds.Contains(x.ServiceTypeId) &&
                x.CanFitService)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Query TechnicianAvailableSlots view for available slots per technician
    /// Business Logic: NONE - Pure data query from materialized view
    /// </summary>
    public async Task<IEnumerable<TechnicianAvailableSlot>> GetTechnicianAvailableSlotsAsync(
        Guid dealershipId,
        DateOnly queryDate,
        CancellationToken cancellationToken)
    {
        return await _dbContext.TechnicianAvailableSlotsView
            .AsNoTracking()
            .Where(x =>
                x.DealershipId == dealershipId &&
                x.QueryDate == queryDate &&
                x.IsAvailable)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Query ServiceBayAvailableSlots view for available slots per service bay
    /// Business Logic: NONE - Pure data query from materialized view
    /// </summary>
    public async Task<IEnumerable<ServiceBayAvailableSlot>> GetServiceBayAvailableSlotsAsync(
        Guid dealershipId,
        DateOnly queryDate,
        CancellationToken cancellationToken)
    {
        return await _dbContext.ServiceBayAvailableSlotsView
            .AsNoTracking()
            .Where(x =>
                x.DealershipId == dealershipId &&
                x.QueryDate == queryDate &&
                x.IsAvailable)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Find an available service bay for a given time slot range
    /// Uses ServiceBayAvailableSlots view to find first available bay that can fit the duration
    /// Business Logic: NONE - Pure data query returning first match
    /// </summary>
    public async Task<ServiceBay?> FindAvailableBayAsync(
        Guid dealershipId,
        int startSlotSequence,
        int endSlotSequence,
        DateOnly appointmentDate,
        CancellationToken cancellationToken)
    {
        // Find available bay slot in the view that covers the entire duration
        var availableBaySlot = await _dbContext.ServiceBayAvailableSlotsView
            .AsNoTracking()
            .Where(x =>
                x.DealershipId == dealershipId &&
                x.QueryDate == appointmentDate &&
                x.SequenceOrder >= startSlotSequence &&
                x.SequenceOrder < endSlotSequence &&
                x.IsAvailable)
            .FirstOrDefaultAsync(cancellationToken);

        if (availableBaySlot == null)
            return null;

        // Load the actual ServiceBay entity
        return await _dbContext.ServiceBays
            .AsNoTracking()
            .Where(b => b.Id == availableBaySlot.ServiceBayId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Get TimeSlot by sequence order
    /// Used to convert sequence order to TimeSlot entities for appointments
    /// Business Logic: NONE - Pure data query
    /// </summary>
    public async Task<TimeSlot?> GetTimeSlotBySequenceAsync(
        int sequenceOrder,
        CancellationToken cancellationToken)
    {
        return await _dbContext.TimeSlots
            .AsNoTracking()
            .Where(t => t.SequenceOrder == sequenceOrder)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Get a range of TimeSlots by sequence order
    /// Used to determine all TimeSlots needed for an appointment of given duration
    /// Business Logic: NONE - Pure data query
    /// </summary>
    public async Task<IEnumerable<TimeSlot>> GetTimeSlotRangeAsync(
        int startSequence,
        int endSequence,
        CancellationToken cancellationToken)
    {
        return await _dbContext.TimeSlots
            .AsNoTracking()
            .Where(t =>
                t.SequenceOrder >= startSequence &&
                t.SequenceOrder <= endSequence)
            .OrderBy(t => t.SequenceOrder)
            .ToListAsync(cancellationToken);
    }
}
