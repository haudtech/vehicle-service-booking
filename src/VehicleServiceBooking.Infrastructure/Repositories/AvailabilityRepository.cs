using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VehicleServiceBooking.Application.Models;
using VehicleServiceBooking.Application.Interfaces.Persistence;
using VehicleServiceBooking.Application.Interfaces.Repositories;
using VehicleServiceBooking.Application.Models.ViewModels;

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

    // ==================== PHASE 4: MATERIALIZED VIEW QUERY METHODS ====================

    /// <summary>
    /// Query ServiceTypeAvailability view for all service types and their available options
    /// Business Logic: NONE - Pure data query from materialized view
    /// Performance: Single database query (< 50ms)
    /// </summary>
    public async Task<IEnumerable<AvailabilityProjection>> GetServiceTypeAvailabilityAsync(
        Guid dealershipId,
        Guid[] serviceTypeIds,
        DateOnly queryDate,
        CancellationToken cancellationToken)
    {
        if (serviceTypeIds.Length == 0)
        {
            return Array.Empty<AvailabilityProjection>();
        }

        var baseQuery = _dbContext.ServiceTypeAvailabilityView
            .AsNoTracking()
            .Where(x =>
                x.DealershipId == dealershipId &&
                x.QueryDate == queryDate &&
                x.CanFitService);

        baseQuery = baseQuery.Where(x => serviceTypeIds.Contains(x.ServiceTypeId));

        // Company-scoped scheduling: exclude candidate windows that overlap with
        // any active service booking for the same technician or service bay,
        // regardless of dealership.
        baseQuery = baseQuery.Where(x =>
            !_dbContext.Services.AsNoTracking().Any(s =>
                s.IsActive &&
                s.BookingDate == queryDate &&
                (
                    (s.TechnicianId.HasValue && s.TechnicianId.Value == x.TechnicianId) ||
                    (s.ServiceBayId.HasValue && s.ServiceBayId.Value == x.ServiceBayId)
                ) &&
                x.SequenceOrder < s.EstimatedEndSlotSequenceExclusive &&
                (x.SequenceOrder + x.RequiredSlots) > s.EstimatedStartSlotSequence));

        var query =
            from x in baseQuery
            join endSlot in _dbContext.TimeSlots.AsNoTracking()
                on x.SequenceOrder + x.RequiredSlots - 1 equals endSlot.SequenceOrder
            where endSlot.IsActive
            select new AvailabilityProjection
            {
                TimeSlotId = x.TimeSlotId,
                EndTimeSlotId = endSlot.Id,
                SlotStartTime = x.SlotStartTime,
                SlotEndTime = endSlot.SlotEndTime,
                TechnicianId = x.TechnicianId,
                ServiceBayId = x.ServiceBayId
            };

        return await query
            .Distinct()
            .OrderBy(x => x.SlotStartTime)
            .ThenBy(x => x.SlotEndTime)
            .ThenBy(x => x.TechnicianId)
            .ThenBy(x => x.ServiceBayId)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Query TechnicianAvailableSlots view for available slots per technician
    /// Business Logic: NONE - Pure data query from materialized view
    /// </summary>
    public async Task<IEnumerable<TechnicianAvailableSlotsView>> GetTechnicianAvailableSlotsAsync(
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
    public async Task<IEnumerable<ServiceBayAvailableSlotsView>> GetServiceBayAvailableSlotsAsync(
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

}
