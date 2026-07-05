using Microsoft.EntityFrameworkCore;
using VehicleServiceBooking.Auth.Data;
using VehicleServiceBooking.Auth.Models;
using VehicleServiceBooking.Auth.Repositories.Interfaces;

namespace VehicleServiceBooking.Auth.Repositories;

/// <summary>
/// Repository implementation for user-role mapping queries and writes.
/// </summary>
public sealed class UserRoleRepository : GenericRepository<UserRole>, IUserRoleRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserRoleRepository"/> class.
    /// </summary>
    public UserRoleRepository(AuthDbContext dbContext)
        : base(dbContext)
    {
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        return await GetQueryable()
            .AnyAsync(x => x.UserId == userId && x.RoleId == roleId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<string>> GetRoleNamesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await GetQueryable()
            .Where(x => x.UserId == userId)
            .Select(x => x.Role.Name)
            .ToListAsync(cancellationToken);
    }
}
