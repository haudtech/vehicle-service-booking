using VehicleServiceBooking.Domain.Entities;

namespace VehicleServiceBooking.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for BusinessHours entity.
/// </summary>
public interface IBusinessHoursRepository : IReadRepository<BusinessHours>, IWriteRepository<BusinessHours>
{
}
