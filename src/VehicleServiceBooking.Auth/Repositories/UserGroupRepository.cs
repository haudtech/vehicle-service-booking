using Microsoft.EntityFrameworkCore;
using VehicleServiceBooking.Auth.Data;
using VehicleServiceBooking.Auth.Models;
using VehicleServiceBooking.Auth.Repositories.Interfaces;

namespace VehicleServiceBooking.Auth.Repositories;

/// <summary>
/// Repository implementation for user-group mapping queries and writes.
/// </summary>
public sealed class UserGroupRepository : GenericRepository<UserGroup>, IUserGroupRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserGroupRepository"/> class.
    /// </summary>
    public UserGroupRepository(AuthDbContext dbContext)
        : base(dbContext)
    {
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(Guid userId, Guid groupId, CancellationToken cancellationToken = default)
    {
        return await GetQueryable()
            .AnyAsync(x => x.UserId == userId && x.GroupId == groupId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<string>> GetRoleNamesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await GetQueryable()
            .Where(x => x.UserId == userId)
            .SelectMany(x => x.Group.GroupRoles.Select(gr => gr.Role.Name))
            .Distinct()
            .ToListAsync(cancellationToken);
    }
}
