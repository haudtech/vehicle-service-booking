using Microsoft.EntityFrameworkCore;
using VehicleServiceBooking.Auth.Data;
using VehicleServiceBooking.Auth.Models;
using VehicleServiceBooking.Auth.Repositories.Interfaces;

namespace VehicleServiceBooking.Auth.Repositories;

/// <summary>
/// Repository implementation for refresh token queries.
/// </summary>
public sealed class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RefreshTokenRepository"/> class.
    /// </summary>
    public RefreshTokenRepository(AuthDbContext dbContext)
        : base(dbContext)
    {
    }

    /// <inheritdoc />
    public Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return GetQueryable().FirstOrDefaultAsync(x => x.Token == token, cancellationToken);
    }

    /// <inheritdoc />
    public Task<RefreshToken?> GetActiveByTokenWithUserAsync(string token, CancellationToken cancellationToken = default)
    {
        return GetQueryable()
            .Include(x => x.User)
            .FirstOrDefaultAsync(
                x => x.Token == token && x.RevokedAt == null && x.ExpiresAt > DateTime.UtcNow,
                cancellationToken);
    }
}
