using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VehicleServiceBooking.Application.Interfaces.Persistence;
using VehicleServiceBooking.Application.Interfaces.Repositories;
using VehicleServiceBooking.Domain.Entities;
using VehicleServiceBooking.Domain.Enums;

namespace VehicleServiceBooking.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for ServiceStatusLookup entity.
/// </summary>
public class ServiceStatusLookupRepository : GenericRepository<ServiceStatusLookup>, IServiceStatusLookupRepository
{
    public ServiceStatusLookupRepository(IApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public virtual async Task<ServiceStatusLookup?> GetByStatusAsync(ServiceStatus status, CancellationToken cancellationToken)
    {
        return await GetQueryable()
            .FirstOrDefaultAsync(x => x.Status == status, cancellationToken);
    }
}
