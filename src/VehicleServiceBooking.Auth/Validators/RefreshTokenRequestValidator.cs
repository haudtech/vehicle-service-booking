using FluentValidation;
using VehicleServiceBooking.Auth.Models.Requests;

namespace VehicleServiceBooking.Auth.Validators;

/// <summary>
/// Validates refresh token request payloads.
/// </summary>
public sealed class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RefreshTokenRequestValidator"/> class.
    /// </summary>
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .MinimumLength(32);
    }
}
