using VehicleServiceBooking.Domain.Entities;

namespace VehicleServiceBooking.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for Service entity.
/// </summary>
public interface IServiceRepository : IReadRepository<Service>, IWriteRepository<Service>
{
}
