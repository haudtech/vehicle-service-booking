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
}
