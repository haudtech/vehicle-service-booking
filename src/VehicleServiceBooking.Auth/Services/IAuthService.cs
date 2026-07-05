using VehicleServiceBooking.Auth.Models.Requests;
using VehicleServiceBooking.Auth.Models.Responses;

namespace VehicleServiceBooking.Auth.Services;

/// <summary>
/// Defines authentication workflows for user access and token lifecycle.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registers a new user and returns issued tokens.
    /// </summary>
    Task<AuthResponse> SignUpAsync(SignUpRequest request, string ipAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Authenticates a user and returns issued tokens.
    /// </summary>
    Task<AuthResponse> LoginAsync(LoginRequest request, string ipAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes tokens using a valid refresh token.
    /// </summary>
    Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, string ipAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes a refresh token.
    /// </summary>
    Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
}
