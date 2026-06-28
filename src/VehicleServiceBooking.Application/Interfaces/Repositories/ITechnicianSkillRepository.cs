using VehicleServiceBooking.Domain.Entities;

namespace VehicleServiceBooking.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for TechnicianSkill entity.
/// </summary>
public interface ITechnicianSkillRepository : IReadRepository<TechnicianSkill>, IWriteRepository<TechnicianSkill>
{
}
