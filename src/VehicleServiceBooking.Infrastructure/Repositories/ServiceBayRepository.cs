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
/// Repository implementation for ServiceBay entity persistence operations
/// Inherits from GenericRepository to leverage common query patterns with AsNoTracking()
/// </summary>
public class ServiceBayRepository : GenericRepository<ServiceBay>, IServiceBayRepository
{
    public ServiceBayRepository(IApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<IEnumerable<ServiceBay>> GetByDealershipAsync(
        Guid dealershipId,
        CancellationToken cancellationToken)
    {
        return await GetQueryable()
            .Where(s => s.DealershipId == dealershipId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ServiceBay>> GetAvailableAsync(
        Guid dealershipId,
        DateTime startTime,
        DateTime endTime,
        CancellationToken cancellationToken)
    {
        // Get service bays that have services scheduled in the time range
        // Check EstimatedStartTime/EstimatedEndTime on Service, not Appointment
        var occupiedBays = await DbContext.DbContext.Set<Service>()
            .AsNoTracking()
            .Where(s => s.DealershipId == dealershipId
                && s.ServiceBayId != null
                && s.EstimatedStartTime != null
                && s.EstimatedEndTime != null
                && s.EstimatedStartTime < endTime
                && s.EstimatedEndTime > startTime)
            .Select(s => s.ServiceBayId)
            .Distinct()
            .ToListAsync(cancellationToken);

        return await GetQueryable()
            .Where(s => s.DealershipId == dealershipId && !occupiedBays.Contains(s.Id))
            .ToListAsync(cancellationToken);
    }
}
