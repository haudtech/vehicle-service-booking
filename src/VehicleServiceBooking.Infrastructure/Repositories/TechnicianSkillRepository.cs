using VehicleServiceBooking.Application.Interfaces.Persistence;
using VehicleServiceBooking.Application.Interfaces.Repositories;
using VehicleServiceBooking.Domain.Entities;

namespace VehicleServiceBooking.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for TechnicianSkill entity.
/// </summary>
public class TechnicianSkillRepository : GenericRepository<TechnicianSkill>, ITechnicianSkillRepository
{
    public TechnicianSkillRepository(IApplicationDbContext dbContext) : base(dbContext)
    {
    }
}
