using VehicleServiceBooking.Auth.Models;

namespace VehicleServiceBooking.Auth.Repositories.Interfaces;

/// <summary>
/// Repository contract for group lookups.
/// </summary>
public interface IGroupRepository : IWriteRepository<Group>
{
    /// <summary>
    /// Gets group id by exact group name.
    /// </summary>
    Task<Guid?> GetGroupIdByNameAsync(string groupName, CancellationToken cancellationToken = default);
}
