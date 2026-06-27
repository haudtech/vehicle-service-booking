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
