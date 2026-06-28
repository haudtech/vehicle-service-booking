using VehicleServiceBooking.Domain.Entities;

namespace VehicleServiceBooking.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for TechnicianSchedule entity.
/// </summary>
public interface ITechnicianScheduleRepository : IReadRepository<TechnicianSchedule>, IWriteRepository<TechnicianSchedule>
{
}
