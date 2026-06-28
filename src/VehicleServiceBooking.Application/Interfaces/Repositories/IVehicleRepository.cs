using VehicleServiceBooking.Domain.Entities;

namespace VehicleServiceBooking.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for Vehicle entity.
/// </summary>
public interface IVehicleRepository : IReadRepository<Vehicle>, IWriteRepository<Vehicle>
{
}
