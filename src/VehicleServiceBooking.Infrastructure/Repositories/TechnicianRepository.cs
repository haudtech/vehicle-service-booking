using VehicleServiceBooking.Application.Interfaces.Persistence;
using VehicleServiceBooking.Application.Interfaces.Repositories;
using VehicleServiceBooking.Domain.Entities;

namespace VehicleServiceBooking.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Technician entity.
/// </summary>
public class TechnicianRepository : GenericRepository<Technician>, ITechnicianRepository
{
    public TechnicianRepository(IApplicationDbContext dbContext) : base(dbContext)
    {
    }
}
