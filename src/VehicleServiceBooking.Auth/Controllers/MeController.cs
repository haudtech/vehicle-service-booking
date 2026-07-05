using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace VehicleServiceBooking.Auth.Controllers;

/// <summary>
/// Provides an authenticated endpoint to inspect caller claims.
/// </summary>
[ApiController]
[Authorize]
[Route("api/v1/me")]
public class MeController : ControllerBase
{
    /// <summary>
    /// Returns claims from the current authenticated principal.
    /// </summary>
    /// <returns>Claim list for the current user.</returns>
    [HttpGet]
    public IActionResult GetMe()
    {
        var claims = User.Claims.Select(claim => new { claim.Type, claim.Value });
        return Ok(new { claims });
    }
}
