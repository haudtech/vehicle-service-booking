using Microsoft.AspNetCore.Mvc;
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

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthController"/> class.
    /// </summary>
    /// <param name="authService">Authentication service.</param>
    public AuthController(IAuthService authService)
    {
        _authService = authService;
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
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return Unauthorized(new { message = ex.Message });
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
}
