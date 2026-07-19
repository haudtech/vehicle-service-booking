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
/// Repository implementation for ServiceType entity.
/// </summary>
public class ServiceTypeRepository : GenericRepository<ServiceType>, IServiceTypeRepository
{
    public ServiceTypeRepository(IApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<ServiceType>> GetByIdsWithPricesAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken)
    {
        var idList = ids.Distinct().ToList();
        if (idList.Count == 0)
        {
            return Array.Empty<ServiceType>();
        }

        return await DbContext.ServiceTypes
            .AsNoTracking()
            .Include(x => x.ServiceTypePrices)
                .ThenInclude(x => x.Currency)
            .Where(x => idList.Contains(x.Id))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}
