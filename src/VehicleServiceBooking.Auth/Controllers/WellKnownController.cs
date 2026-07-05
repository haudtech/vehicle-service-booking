using Microsoft.AspNetCore.Mvc;
using VehicleServiceBooking.Auth.Services;

namespace VehicleServiceBooking.Auth.Controllers;

/// <summary>
/// Exposes well-known metadata endpoints.
/// </summary>
[ApiController]
[Route("api/v1")]
public class WellKnownController : ControllerBase
{
    private readonly ISigningKeyProvider _signingKeyProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="WellKnownController"/> class.
    /// </summary>
    /// <param name="signingKeyProvider">Signing key provider for JWKS generation.</param>
    public WellKnownController(ISigningKeyProvider signingKeyProvider)
    {
        _signingKeyProvider = signingKeyProvider;
    }

    /// <summary>
    /// Returns the current JSON Web Key Set for JWT validation.
    /// </summary>
    /// <returns>JWKS document.</returns>
    [HttpGet(".well-known/jwks.json")]
    public IActionResult GetJwks()
    {
        return Ok(_signingKeyProvider.GetPublicJwks());
    }
}
