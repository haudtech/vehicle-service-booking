using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VehicleServiceBooking.Application.DTOs;
using VehicleServiceBooking.Application.Interfaces.Services;

namespace VehicleServiceBooking.Api.Controllers;

/// <summary>
/// Provides customer profile endpoints.
/// </summary>
[ApiController]
[Route("api/v1/customers")]
[Authorize]
public sealed class CustomersController : ControllerBase
{
    private readonly ICustomerIdentityService _customerIdentityService;

    public CustomersController(ICustomerIdentityService customerIdentityService)
    {
        _customerIdentityService = customerIdentityService ?? throw new ArgumentNullException(nameof(customerIdentityService));
    }

    /// <summary>
    /// Gets current caller customer profile and lazily bootstraps it from Auth profile when missing.
    /// </summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(CustomerProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetMe(CancellationToken cancellationToken)
    {
        var authUserIdClaim = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (!Guid.TryParse(authUserIdClaim, out var authUserId))
        {
            return Unauthorized(new { message = "Authenticated user id claim is missing or invalid." });
        }

        try
        {
            var customer = await _customerIdentityService
                .GetOrCreateCustomerByAuthUserIdAsync(authUserId, cancellationToken)
                .ConfigureAwait(false);

            var response = new CustomerProfileResponse
            {
                CustomerId = customer.Id,
                AuthUserId = customer.AuthUserId,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                CreatedAtUtc = customer.CreatedAt,
                UpdatedAtUtc = customer.UpdatedAt
            };

            return Ok(response);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new ErrorResponse
            {
                Message = "Unable to bootstrap customer profile from Auth service.",
                ErrorCode = "CUSTOMER_BOOTSTRAP_UNAVAILABLE",
                Timestamp = DateTime.UtcNow
            });
        }
    }
}