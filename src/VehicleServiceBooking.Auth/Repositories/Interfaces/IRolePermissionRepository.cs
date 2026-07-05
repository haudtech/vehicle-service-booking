namespace VehicleServiceBooking.Auth.Repositories.Interfaces;

/// <summary>
/// Repository contract for permission lookup by role assignment.
/// </summary>
public interface IRolePermissionRepository
{
    /// <summary>
    /// Gets permission names granted to a user through role mappings.
    /// </summary>
    Task<IReadOnlyCollection<string>> GetPermissionNamesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
