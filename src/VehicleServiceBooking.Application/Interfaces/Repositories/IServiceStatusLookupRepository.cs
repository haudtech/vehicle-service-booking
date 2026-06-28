using System.Threading;
using System.Threading.Tasks;
using VehicleServiceBooking.Domain.Entities;
using VehicleServiceBooking.Domain.Enums;

namespace VehicleServiceBooking.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for ServiceStatusLookup entity.
/// </summary>
public interface IServiceStatusLookupRepository : IReadRepository<ServiceStatusLookup>
{
    Task<ServiceStatusLookup?> GetByStatusAsync(ServiceStatus status, CancellationToken cancellationToken);
}
