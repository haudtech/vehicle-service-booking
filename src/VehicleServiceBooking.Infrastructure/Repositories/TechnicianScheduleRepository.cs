using VehicleServiceBooking.Application.Interfaces.Persistence;
using VehicleServiceBooking.Application.Interfaces.Repositories;
using VehicleServiceBooking.Domain.Entities;

namespace VehicleServiceBooking.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for TechnicianSchedule entity.
/// </summary>
public class TechnicianScheduleRepository : GenericRepository<TechnicianSchedule>, ITechnicianScheduleRepository
{
    public TechnicianScheduleRepository(IApplicationDbContext dbContext) : base(dbContext)
    {
    }
}
