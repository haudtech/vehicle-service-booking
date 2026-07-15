using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VehicleServiceBooking.Auth.Common.Enums;
using VehicleServiceBooking.Auth.Configuration;
using VehicleServiceBooking.Auth.Models.Notifications;
using VehicleServiceBooking.Auth.Models.Requests;
using VehicleServiceBooking.Auth.Services;

namespace VehicleServiceBooking.Auth.Controllers;

/// <summary>
/// Provides authentication endpoints for sign-up, login, refresh, and logout.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly GoogleAuthOptions _googleAuthOptions;
    private readonly NotificationOptions _notificationOptions;
    private readonly INotificationPublisher _notificationPublisher;
    private readonly ILogger<AuthController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthController"/> class.
    /// </summary>
    /// <param name="authService">Authentication service.</param>
    public AuthController(
        IAuthService authService,
        GoogleAuthOptions googleAuthOptions,
        NotificationOptions notificationOptions,
        INotificationPublisher notificationPublisher,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _googleAuthOptions = googleAuthOptions;
        _notificationOptions = notificationOptions;
        _notificationPublisher = notificationPublisher;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new user account and returns access and refresh tokens.
    /// </summary>
    /// <param name="request">Sign-up payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created response with token payload or a conflict response.</returns>
    [HttpPost("signup")]
    public async Task<IActionResult> SignUp([FromBody] SignUpRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _authService.SignUpAsync(request, HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown", cancellationToken);
            var verificationLink = BuildEmailVerificationLink(response.Email, response.VerificationToken);
            await PublishNotificationSafeAsync(
                eventType: "Auth.EmailVerification.Requested",
                toEmail: request.Email,
                subject: "Verify your email address",
                content: $"Please verify your email address.\n\nOpen this link: {verificationLink}",
                htmlContent: BuildEmailVerificationHtml(verificationLink),
                cancellationToken);

            return Accepted(new
            {
                message = "Sign-up successful. Please verify your email before signing in.",
                email = response.Email,
                verificationLink,
                verificationTokenExpiresAtUtc = response.VerificationTokenExpiresAtUtc
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Verifies email using callback link query parameters.
    /// </summary>
    /// <param name="email">Email address to verify.</param>
    /// <param name="token">One-time verification token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success response when verification completes.</returns>
    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmailCallback([FromQuery] string email, [FromQuery] string token, CancellationToken cancellationToken)
    {
        try
        {
            await _authService.VerifyEmailAsync(new VerifyEmailRequest
            {
                Email = email,
                Token = token
            }, cancellationToken);

            await PublishNotificationSafeAsync(
                eventType: "Auth.EmailVerification.Success",
                toEmail: email,
                subject: "Email verified successfully",
                content: "Your account email has been verified. You can now sign in.",
                htmlContent: null,
                cancellationToken);

            return Ok(new { message = "Email verified successfully." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Verifies ownership of an email address for a pending account.
    /// </summary>
    /// <param name="request">Verify-email payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No-content response when verification succeeds.</returns>
    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _authService.VerifyEmailAsync(request, cancellationToken);

            await PublishNotificationSafeAsync(
                eventType: "Auth.EmailVerification.Success",
                toEmail: request.Email,
                subject: "Email verified successfully",
                content: "Your account email has been verified. You can now sign in.",
                htmlContent: null,
                cancellationToken);

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Authenticates credentials and starts login verification challenge.
    /// </summary>
    /// <param name="request">Login payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Accepted response requiring verification code submission.</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _authService.StartLoginChallengeAsync(request, cancellationToken);
            if (response.ChallengeChannel == ChallengeChannel.EmailOtp)
            {
                await PublishNotificationSafeAsync(
                    eventType: "Auth.Login.VerificationCode.Requested",
                    toEmail: response.Email,
                    subject: "Your login verification code",
                    content: $"Use this code to complete login: {response.VerificationCode}",
                    htmlContent: null,
                    cancellationToken);
            }

            return Accepted(new
            {
                message = response.ChallengeChannel == ChallengeChannel.AuthenticatorApp
                    ? "Credentials accepted. Open your authenticator app and submit the current OTP code."
                    : "Credentials accepted. Submit the verification code sent to your email.",
                email = response.Email,
                challengeId = response.ChallengeId,
                challengeChannel = response.ChallengeChannel.ToWireValue(),
                verificationCodeExpiresAtUtc = response.VerificationCodeExpiresAtUtc
            });
        }
        catch (InvalidOperationException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Completes login verification challenge and returns access and refresh tokens.
    /// </summary>
    /// <param name="request">Login code verification payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Token payload or unauthorized response.</returns>
    [HttpPost("login/verify-code")]
    public async Task<IActionResult> VerifyLoginCode([FromBody] LoginVerifyCodeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _authService.VerifyLoginCodeAsync(
                request,
                HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                cancellationToken);

            await PublishNotificationSafeAsync(
                eventType: "Auth.Login.Success",
                toEmail: request.Email,
                subject: "New sign-in detected",
                content: "Your account has signed in successfully.",
                htmlContent: null,
                cancellationToken);

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Starts authenticator-app enrollment for the current user.
    /// Purpose: provide otpauth URI/QR payload so the client can scan and register the account in an authenticator app.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Enrollment material including otpauth URI and masked secret.</returns>
    [Authorize]
    [HttpPost("mfa/authenticator/setup/start")]
    public async Task<IActionResult> StartAuthenticatorSetup(CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized(new { message = "Authenticated user id claim is missing or invalid." });
        }

        try
        {
            var response = await _authService.StartAuthenticatorSetupAsync(userId, cancellationToken);
            response.QrImageUrl = $"{Request.Scheme}://{Request.Host}/api/v1/auth/mfa/authenticator/setup/qr";
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Returns QR PNG image for the current user's pending authenticator-app setup.
    /// Purpose: allow clients to display/scannable QR without external QR generation tools.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>PNG image bytes for authenticator enrollment.</returns>
    [Authorize]
    [HttpGet("mfa/authenticator/setup/qr")]
    public async Task<IActionResult> GetAuthenticatorSetupQrCode(CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized(new { message = "Authenticated user id claim is missing or invalid." });
        }

        try
        {
            var png = await _authService.GetAuthenticatorSetupQrCodePngAsync(userId, cancellationToken);
            return File(png, "image/png");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Verifies authenticator-app enrollment for the current user.
    /// Purpose: confirm possession of the authenticator secret using a valid TOTP, then enable authenticator-based login challenges.
    /// </summary>
    /// <param name="request">Current TOTP code from authenticator app.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Enabled status and active login verification channel.</returns>
    [Authorize]
    [HttpPost("mfa/authenticator/setup/verify")]
    public async Task<IActionResult> VerifyAuthenticatorSetup([FromBody] AuthenticatorSetupVerifyRequest request, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized(new { message = "Authenticated user id claim is missing or invalid." });
        }

        try
        {
            var response = await _authService.VerifyAuthenticatorSetupAsync(userId, request, cancellationToken);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Starts Google OAuth challenge flow.
    /// </summary>
    /// <returns>Authentication challenge response.</returns>
    [HttpGet("google/start")]
    public IActionResult StartGoogleLogin()
    {
        if (!_googleAuthOptions.Enabled)
        {
            return Conflict(new
            {
                message = "Google login is disabled by configuration.",
                code = "GOOGLE_LOGIN_DISABLED"
            });
        }

        var callbackUrl = $"{Request.Scheme}://{Request.Host}/api/v1/auth/google/callback";

        var properties = new AuthenticationProperties
        {
            RedirectUri = callbackUrl
        };

        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    /// <summary>
    /// Handles Google OAuth callback and issues local access/refresh tokens.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Issued token payload or authorization error response.</returns>
    [HttpGet("google/callback")]
    public async Task<IActionResult> GoogleCallback(CancellationToken cancellationToken)
    {
        if (!_googleAuthOptions.Enabled)
        {
            return Conflict(new
            {
                message = "Google login is disabled by configuration.",
                code = "GOOGLE_LOGIN_DISABLED"
            });
        }

        var externalResult = await HttpContext.AuthenticateAsync(GoogleAuthOptions.ExternalCookieScheme);
        if (externalResult is null || !externalResult.Succeeded || externalResult.Principal is null)
        {
            return Unauthorized(new { message = "Google authentication failed." });
        }

        try
        {
            var email = externalResult.Principal.FindFirstValue(ClaimTypes.Email)
                ?? externalResult.Principal.FindFirstValue("email");

            if (string.IsNullOrWhiteSpace(email))
            {
                return Unauthorized(new { message = "Google identity did not return an email address." });
            }

            var providerSubject = externalResult.Principal.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? externalResult.Principal.FindFirstValue("sub");

            if (string.IsNullOrWhiteSpace(providerSubject))
            {
                return Unauthorized(new { message = "Google identity is missing provider subject." });
            }

            var displayName = externalResult.Principal.FindFirstValue(ClaimTypes.Name);
            var response = await _authService.LoginWithGoogleAsync(
                email,
                displayName,
                providerSubject,
                ReadGoogleEmailVerifiedClaim(externalResult.Principal, externalResult.Properties),
                HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                cancellationToken);

            await PublishNotificationSafeAsync(
                eventType: "Auth.GoogleLogin.Success",
                toEmail: email,
                subject: "Google sign-in successful",
                content: "You have signed in successfully using Google.",
                htmlContent: null,
                cancellationToken);

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        finally
        {
            await HttpContext.SignOutAsync(GoogleAuthOptions.ExternalCookieScheme);
        }
    }

    /// <summary>
    /// Exchanges a valid refresh token for a new access and refresh token pair.
    /// </summary>
    /// <param name="request">Refresh token payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Token payload or unauthorized response.</returns>
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _authService.RefreshTokenAsync(request, HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown", cancellationToken);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Revokes a refresh token.
    /// </summary>
    /// <param name="request">Refresh token payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No-content response.</returns>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        await _authService.RevokeRefreshTokenAsync(request.RefreshToken, cancellationToken);
        return NoContent();
    }

    private async Task PublishNotificationSafeAsync(
        string eventType,
        string? toEmail,
        string subject,
        string content,
        string? htmlContent,
        CancellationToken cancellationToken)
    {
        if (!_notificationOptions.Enabled)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(toEmail))
        {
            _logger.LogWarning("Notification publish skipped for {EventType}: target email is missing.", eventType);
            return;
        }

        using var publishCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        publishCts.CancelAfter(_notificationOptions.PublishTimeout);

        try
        {
            await _notificationPublisher.PublishAsync(new NotificationMessage
            {
                EventType = eventType,
                ToEmail = toEmail.Trim(),
                Subject = subject,
                Content = content,
                HtmlContent = htmlContent,
                CorrelationId = HttpContext.TraceIdentifier,
                Source = string.IsNullOrWhiteSpace(_notificationOptions.Source) ? "auth-service" : _notificationOptions.Source,
                OccurredAtUtc = DateTime.UtcNow
            }, publishCts.Token);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning(
                "Notification publish timed out for {EventType} and email {Email} after {Timeout}.",
                eventType,
                toEmail,
                _notificationOptions.PublishTimeout);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Notification publish failed for {EventType} and email {Email}.", eventType, toEmail);
        }
    }

    private string BuildEmailVerificationLink(string email, string token)
    {
        var callbackUrl = Url.ActionLink(
            nameof(VerifyEmailCallback),
            values: new { email, token });

        return callbackUrl ?? $"{Request.Scheme}://{Request.Host}/api/v1/auth/verify-email?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}";
    }

    private static bool ReadGoogleEmailVerifiedClaim(ClaimsPrincipal principal, AuthenticationProperties? properties)
    {
        var raw = principal.FindFirstValue("email_verified")
            ?? principal.FindFirstValue("urn:google:email_verified")
            ?? principal.FindFirstValue("verified_email")
            ?? ReadEmailVerifiedFromIdToken(properties?.GetTokenValue("id_token"));

        if (string.IsNullOrWhiteSpace(raw))
        {
            return false;
        }

        if (bool.TryParse(raw, out var parsedBool))
        {
            return parsedBool;
        }

        return raw == "1";
    }

    private static string? ReadEmailVerifiedFromIdToken(string? idToken)
    {
        if (string.IsNullOrWhiteSpace(idToken))
        {
            return null;
        }

        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(idToken))
        {
            return null;
        }

        var token = handler.ReadJwtToken(idToken);
        return token.Claims.FirstOrDefault(claim =>
            claim.Type == "email_verified" ||
            claim.Type == "verified_email")?.Value;
    }

    private static string BuildEmailVerificationHtml(string verificationLink)
    {
        var encodedLink = System.Net.WebUtility.HtmlEncode(verificationLink);
        return $"<p>Please verify your email address.</p><p><a href=\"{encodedLink}\">Verify your email address</a></p>";
    }

    private bool TryGetCurrentUserId(out Guid userId)
    {
        var claimValue = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        return Guid.TryParse(claimValue, out userId);
    }
}
