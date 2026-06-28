using VehicleServiceBooking.Application.Interfaces.Persistence;
using VehicleServiceBooking.Application.Interfaces.Repositories;
using VehicleServiceBooking.Domain.Entities;

namespace VehicleServiceBooking.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Dealership entity.
/// </summary>
public class DealershipRepository : GenericRepository<Dealership>, IDealershipRepository
{
    public DealershipRepository(IApplicationDbContext dbContext) : base(dbContext)
    {
    }
}
