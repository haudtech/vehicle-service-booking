namespace VehicleServiceBooking.Auth.Repositories.Interfaces;

/// <summary>
/// Repository contract for role lookups.
/// </summary>
public interface IRoleRepository
{
    /// <summary>
    /// Gets a role identifier by role name.
    /// </summary>
    Task<Guid?> GetRoleIdByNameAsync(string roleName, CancellationToken cancellationToken = default);
}
