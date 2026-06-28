using VehicleServiceBooking.Domain.Entities;

namespace VehicleServiceBooking.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for Technician entity.
/// </summary>
public interface ITechnicianRepository : IReadRepository<Technician>, IWriteRepository<Technician>
{
}
