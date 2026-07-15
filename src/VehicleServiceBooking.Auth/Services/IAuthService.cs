using VehicleServiceBooking.Auth.Models.Requests;
using VehicleServiceBooking.Auth.Models.Responses;

namespace VehicleServiceBooking.Auth.Services;

/// <summary>
/// Defines authentication workflows for user access and token lifecycle.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registers a new user and returns email verification details.
    /// </summary>
    Task<SignUpResult> SignUpAsync(SignUpRequest request, string ipAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies a pending user email using a proof token.
    /// </summary>
    Task VerifyEmailAsync(VerifyEmailRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Starts a login challenge after validating user credentials.
    /// </summary>
    Task<LoginChallengeResult> StartLoginChallengeAsync(LoginRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Starts authenticator-app setup for an authenticated user by generating enrollment material.
    /// </summary>
    Task<AuthenticatorSetupStartResult> StartAuthenticatorSetupAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies authenticator-app setup with a current TOTP code and activates authenticator login channel.
    /// </summary>
    Task<AuthenticatorSetupVerifyResult> VerifyAuthenticatorSetupAsync(Guid userId, AuthenticatorSetupVerifyRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a PNG QR code image for the current user's pending authenticator setup.
    /// </summary>
    Task<byte[]> GetAuthenticatorSetupQrCodePngAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Completes a login challenge and returns issued tokens when code verification succeeds.
    /// </summary>
    Task<AuthResponse> VerifyLoginCodeAsync(LoginVerifyCodeRequest request, string ipAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Authenticates or provisions a user from Google identity and returns issued tokens.
    /// </summary>
    Task<AuthResponse> LoginWithGoogleAsync(string email, string? displayName, string providerUserId, bool emailVerified, string ipAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes tokens using a valid refresh token.
    /// </summary>
    Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, string ipAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes a refresh token.
    /// </summary>
    Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
}
