using Microsoft.AspNetCore.Mvc;
using VehicleServiceBooking.Auth.Configuration;
using VehicleServiceBooking.Auth.Services;

namespace VehicleServiceBooking.Auth.Controllers;

/// <summary>
/// Internal key-management endpoints for non-production key rotation workflows.
/// </summary>
[ApiController]
[Route("api/v1/internal/keys")]
public class KeyManagementController : ControllerBase
{
    private readonly ISigningKeyProvider _signingKeyProvider;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly KeyRotationOptions _keyRotationOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyManagementController"/> class.
    /// </summary>
    public KeyManagementController(
        ISigningKeyProvider signingKeyProvider,
        IHostEnvironment hostEnvironment,
        KeyRotationOptions keyRotationOptions)
    {
        _signingKeyProvider = signingKeyProvider;
        _hostEnvironment = hostEnvironment;
        _keyRotationOptions = keyRotationOptions;
    }

    /// <summary>
    /// Rotates the active JWT signing key in Development/Staging environments.
    /// </summary>
    /// <returns>Rotation result including active key id.</returns>
    [HttpPost("rotate")]
    public IActionResult Rotate()
    {
        if (!_hostEnvironment.IsDevelopment() && !_hostEnvironment.IsStaging())
        {
            return NotFound();
        }

        if (!_keyRotationOptions.EnableRotation)
        {
            return Conflict(new
            {
                message = "Key rotation is disabled by configuration.",
                code = "KEY_ROTATION_DISABLED"
            });
        }

        var newKid = _signingKeyProvider.RotateKey();
        return Ok(new
        {
            activeKid = newKid,
            rotatedAtUtc = DateTime.UtcNow
        });
    }
}
