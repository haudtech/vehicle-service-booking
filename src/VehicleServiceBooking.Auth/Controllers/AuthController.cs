using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
            await PublishNotificationSafeAsync(
                eventType: "Auth.SignUp.Success",
                toEmail: request.Email,
                subject: "Welcome to Vehicle Service Booking",
                content: "Your account was created successfully.",
                cancellationToken);

            return Created(string.Empty, response);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Authenticates an existing user and returns access and refresh tokens.
    /// </summary>
    /// <param name="request">Login payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Token payload or unauthorized response.</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _authService.LoginAsync(request, HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown", cancellationToken);
            var email = ResolveLoginEmail(request);
            await PublishNotificationSafeAsync(
                eventType: "Auth.Login.Success",
                toEmail: email,
                subject: "New sign-in detected",
                content: "Your account has signed in successfully.",
                cancellationToken);

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return Unauthorized(new { message = ex.Message });
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
                HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                cancellationToken);

            await PublishNotificationSafeAsync(
                eventType: "Auth.GoogleLogin.Success",
                toEmail: email,
                subject: "Google sign-in successful",
                content: "You have signed in successfully using Google.",
                cancellationToken);

            return Ok(response);
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

    private static string? ResolveLoginEmail(LoginRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            return request.Email;
        }

        if (!string.IsNullOrWhiteSpace(request.Identifier) && request.Identifier.Contains('@'))
        {
            return request.Identifier;
        }

        return null;
    }
}
