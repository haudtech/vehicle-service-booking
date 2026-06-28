using VehicleServiceBooking.Application.Interfaces.Persistence;
using VehicleServiceBooking.Application.Interfaces.Repositories;
using VehicleServiceBooking.Domain.Entities;

namespace VehicleServiceBooking.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Service entity.
/// </summary>
public class ServiceRepository : GenericRepository<Service>, IServiceRepository
{
    public ServiceRepository(IApplicationDbContext dbContext) : base(dbContext)
    {
    }
}
