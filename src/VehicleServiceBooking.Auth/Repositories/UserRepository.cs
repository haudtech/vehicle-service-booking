using Microsoft.EntityFrameworkCore;
using VehicleServiceBooking.Auth.Data;
using VehicleServiceBooking.Auth.Models;
using VehicleServiceBooking.Auth.Repositories.Interfaces;

namespace VehicleServiceBooking.Auth.Repositories;

/// <summary>
/// Repository implementation for user persistence and authorization-aware lookups.
/// </summary>
public sealed class UserRepository : GenericRepository<User>, IUserRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserRepository"/> class.
    /// </summary>
    public UserRepository(AuthDbContext dbContext)
        : base(dbContext)
    {
    }

    /// <inheritdoc />
    public Task<User?> GetByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
    {
        return GetQueryable().FirstOrDefaultAsync(x => x.Email == normalizedEmail, cancellationToken);
    }

    /// <inheritdoc />
    public Task<User?> GetByAccountNameAsync(string normalizedAccountName, CancellationToken cancellationToken = default)
    {
        return GetQueryable().FirstOrDefaultAsync(x => x.AccountName == normalizedAccountName, cancellationToken);
    }

    /// <inheritdoc />
    public Task<User?> GetActiveByEmailWithAuthorizationAsync(string normalizedEmail, CancellationToken cancellationToken = default)
    {
        return GetQueryable()
            .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                    .ThenInclude(x => x.RolePermissions)
                        .ThenInclude(x => x.Permission)
            .FirstOrDefaultAsync(x => x.Email == normalizedEmail, cancellationToken);
    }

    /// <inheritdoc />
    public Task<User?> GetActiveByEmailOrAccountNameWithAuthorizationAsync(string normalizedIdentifier, CancellationToken cancellationToken = default)
    {
        return GetQueryable()
            .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                    .ThenInclude(x => x.RolePermissions)
                        .ThenInclude(x => x.Permission)
            .FirstOrDefaultAsync(
                x => x.Email == normalizedIdentifier || x.AccountName == normalizedIdentifier,
                cancellationToken);
    }
}
