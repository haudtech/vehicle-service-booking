using VehicleServiceBooking.Application.Interfaces.Persistence;
using VehicleServiceBooking.Application.Interfaces.Repositories;
using VehicleServiceBooking.Domain.Entities;

namespace VehicleServiceBooking.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for AppointmentStatusLookup entity.
/// </summary>
public class AppointmentStatusLookupRepository : GenericRepository<AppointmentStatusLookup>, IAppointmentStatusLookupRepository
{
    public AppointmentStatusLookupRepository(IApplicationDbContext dbContext) : base(dbContext)
    {
    }
}
