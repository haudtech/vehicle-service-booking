using VehicleServiceBooking.Application.Interfaces.Persistence;
using VehicleServiceBooking.Application.Interfaces.Repositories;
using VehicleServiceBooking.Domain.Entities;

namespace VehicleServiceBooking.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for ServiceStatusLookup entity.
/// </summary>
public class ServiceStatusLookupRepository : GenericRepository<ServiceStatusLookup>, IServiceStatusLookupRepository
{
    public ServiceStatusLookupRepository(IApplicationDbContext dbContext) : base(dbContext)
    {
    }
}
