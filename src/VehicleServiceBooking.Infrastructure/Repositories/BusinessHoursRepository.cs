using VehicleServiceBooking.Application.Interfaces.Persistence;
using VehicleServiceBooking.Application.Interfaces.Repositories;
using VehicleServiceBooking.Domain.Entities;

namespace VehicleServiceBooking.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for BusinessHours entity.
/// </summary>
public class BusinessHoursRepository : GenericRepository<BusinessHours>, IBusinessHoursRepository
{
    public BusinessHoursRepository(IApplicationDbContext dbContext) : base(dbContext)
    {
    }
}
