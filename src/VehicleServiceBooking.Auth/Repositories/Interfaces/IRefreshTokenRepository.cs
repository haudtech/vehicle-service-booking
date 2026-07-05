using VehicleServiceBooking.Auth.Models;

namespace VehicleServiceBooking.Auth.Repositories.Interfaces;

/// <summary>
/// Repository contract for refresh token persistence and lookups.
/// </summary>
public interface IRefreshTokenRepository : IReadRepository<RefreshToken>, IWriteRepository<RefreshToken>
{
    /// <summary>
    /// Gets a refresh token by token value.
    /// </summary>
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an active refresh token by token value including user data.
    /// </summary>
    Task<RefreshToken?> GetActiveByTokenWithUserAsync(string token, CancellationToken cancellationToken = default);
}
