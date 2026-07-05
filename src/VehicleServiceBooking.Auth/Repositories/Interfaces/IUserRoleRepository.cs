using VehicleServiceBooking.Auth.Models;

namespace VehicleServiceBooking.Auth.Repositories.Interfaces;

/// <summary>
/// Repository contract for user-role mapping operations.
/// </summary>
public interface IUserRoleRepository : IWriteRepository<UserRole>
{
    /// <summary>
    /// Checks whether a user-role mapping exists.
    /// </summary>
    Task<bool> ExistsAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets role names assigned to a user.
    /// </summary>
    Task<IReadOnlyCollection<string>> GetRoleNamesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
