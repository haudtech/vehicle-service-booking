using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VehicleServiceBooking.Auth.Services;

namespace VehicleServiceBooking.Auth.Controllers;

/// <summary>
/// Internal user endpoints for service-to-service profile lookup.
/// </summary>
[ApiController]
[Authorize]
[Route("api/v1/internal/users")]
public sealed class InternalUsersController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<InternalUsersController> _logger;

    public InternalUsersController(IAuthService authService, ILogger<InternalUsersController> logger)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Returns core profile fields for the requested auth user identifier.
    /// </summary>
    [HttpGet("{authUserId:guid}/core-profile")]
    public async Task<IActionResult> GetCoreProfileById(Guid authUserId, CancellationToken cancellationToken)
    {
        try
        {
            var profile = await _authService.GetUserCoreProfileByIdAsync(authUserId, cancellationToken);
            if (profile is null)
            {
                return NotFound(new { message = "User not found." });
            }

            if (!profile.IsActive || !profile.IsEmailVerified)
            {
                return Conflict(new { message = "User is not eligible for booking bootstrap." });
            }

            return Ok(profile);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning(
                ex,
                "Core profile lookup was canceled for authUserId={AuthUserId}.",
                authUserId);

            return StatusCode(499, new { message = "Request canceled." });
        }
    }
}