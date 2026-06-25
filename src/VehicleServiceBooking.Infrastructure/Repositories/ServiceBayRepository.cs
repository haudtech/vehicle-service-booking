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
/// </summary>
public class ServiceBayRepository : IServiceBayRepository
{
    private readonly IApplicationDbContext _dbContext;

    public ServiceBayRepository(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<ServiceBay?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.ServiceBays
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<ServiceBay>> GetByDealershipAsync(
        Guid dealershipId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.ServiceBays
            .Where(s => s.DealershipId == dealershipId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ServiceBay>> GetAvailableAsync(
        Guid dealershipId,
        DateTime startTime,
        DateTime endTime,
        CancellationToken cancellationToken)
    {
        var occupiedBays = await _dbContext.Appointments
            .Where(a => a.DealershipId == dealershipId
                && a.StartTime < endTime
                && a.EndTime > startTime)
            .Select(a => a.ServiceBayId)
            .Distinct()
            .ToListAsync(cancellationToken);

        return await _dbContext.ServiceBays
            .Where(s => s.DealershipId == dealershipId && !occupiedBays.Contains(s.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<ServiceBay> AddAsync(ServiceBay serviceBay, CancellationToken cancellationToken)
    {
        _dbContext.ServiceBays.Add(serviceBay);
        await SaveChangesAsync(cancellationToken);
        return serviceBay;
    }

    public async Task<ServiceBay> UpdateAsync(ServiceBay serviceBay, CancellationToken cancellationToken)
    {
        _dbContext.ServiceBays.Update(serviceBay);
        await SaveChangesAsync(cancellationToken);
        return serviceBay;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var serviceBay = await GetByIdAsync(id, cancellationToken);
        if (serviceBay != null)
        {
            _dbContext.ServiceBays.Remove(serviceBay);
            await SaveChangesAsync(cancellationToken);
        }
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
