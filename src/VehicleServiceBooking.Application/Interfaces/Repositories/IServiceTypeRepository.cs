using VehicleServiceBooking.Domain.Entities;

namespace VehicleServiceBooking.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for ServiceType entity.
/// </summary>
public interface IServiceTypeRepository : IReadRepository<ServiceType>, IWriteRepository<ServiceType>
{
}
