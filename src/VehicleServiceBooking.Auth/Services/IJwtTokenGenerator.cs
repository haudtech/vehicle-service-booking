using VehicleServiceBooking.Auth.Models;

namespace VehicleServiceBooking.Auth.Services;

/// <summary>
/// Defines JWT generation behavior for authenticated users.
/// </summary>
public interface IJwtTokenGenerator
{
    /// <summary>
    /// Generates an access token for the specified user context.
    /// </summary>
    string GenerateAccessToken(User user, IReadOnlyCollection<string> roles, IReadOnlyCollection<string> permissions);
}
