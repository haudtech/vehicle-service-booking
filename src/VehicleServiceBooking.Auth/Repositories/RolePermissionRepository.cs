using Microsoft.EntityFrameworkCore;
using VehicleServiceBooking.Auth.Data;
using VehicleServiceBooking.Auth.Models;
using VehicleServiceBooking.Auth.Repositories.Interfaces;

namespace VehicleServiceBooking.Auth.Repositories;

/// <summary>
/// Repository implementation for permission lookups by role membership.
/// </summary>
public sealed class RolePermissionRepository : GenericRepository<RolePermission>, IRolePermissionRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RolePermissionRepository"/> class.
    /// </summary>
    public RolePermissionRepository(AuthDbContext dbContext)
        : base(dbContext)
    {
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<string>> GetPermissionNamesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await GetQueryable()
            .Where(x => x.Role.GroupRoles.Any(gr => gr.Group.UserGroups.Any(ug => ug.UserId == userId)))
            .Select(x => x.Permission.Name)
            .Distinct()
            .ToListAsync(cancellationToken);
    }
}
