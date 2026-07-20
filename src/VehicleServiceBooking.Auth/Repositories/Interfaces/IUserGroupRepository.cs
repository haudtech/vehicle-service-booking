using VehicleServiceBooking.Auth.Models;

namespace VehicleServiceBooking.Auth.Repositories.Interfaces;

/// <summary>
/// Repository contract for user-group mapping operations.
/// </summary>
public interface IUserGroupRepository : IWriteRepository<UserGroup>
{
    /// <summary>
    /// Checks whether a user-group mapping exists.
    /// </summary>
    Task<bool> ExistsAsync(Guid userId, Guid groupId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets role names granted to a user through group-role mappings.
    /// </summary>
    Task<IReadOnlyCollection<string>> GetRoleNamesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
