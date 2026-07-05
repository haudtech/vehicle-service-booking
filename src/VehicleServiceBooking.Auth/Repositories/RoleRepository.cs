using Microsoft.EntityFrameworkCore;
using VehicleServiceBooking.Auth.Data;
using VehicleServiceBooking.Auth.Models;
using VehicleServiceBooking.Auth.Repositories.Interfaces;

namespace VehicleServiceBooking.Auth.Repositories;

/// <summary>
/// Repository implementation for role lookups.
/// </summary>
public sealed class RoleRepository : GenericRepository<Role>, IRoleRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RoleRepository"/> class.
    /// </summary>
    public RoleRepository(AuthDbContext dbContext)
        : base(dbContext)
    {
    }

    /// <inheritdoc />
    public async Task<Guid?> GetRoleIdByNameAsync(string roleName, CancellationToken cancellationToken = default)
    {
        var roleId = await GetQueryable()
            .Where(x => x.Name == roleName)
            .Select(x => (Guid?)x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return roleId;
    }
}
