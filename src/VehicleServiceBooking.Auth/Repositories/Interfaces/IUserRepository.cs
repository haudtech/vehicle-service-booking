using VehicleServiceBooking.Auth.Models;

namespace VehicleServiceBooking.Auth.Repositories.Interfaces;

/// <summary>
/// Repository contract for user persistence and retrieval.
/// </summary>
public interface IUserRepository : IReadRepository<User>, IWriteRepository<User>
{
    /// <summary>
    /// Gets a user by normalized email.
    /// </summary>
    Task<User?> GetByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user by normalized account name.
    /// </summary>
    Task<User?> GetByAccountNameAsync(string normalizedAccountName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an active user by email with authorization graph loaded.
    /// </summary>
    Task<User?> GetActiveByEmailWithAuthorizationAsync(string normalizedEmail, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an active user by email or account name with authorization graph loaded.
    /// </summary>
    Task<User?> GetActiveByEmailOrAccountNameWithAuthorizationAsync(string normalizedIdentifier, CancellationToken cancellationToken = default);
}
