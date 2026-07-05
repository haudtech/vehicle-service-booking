using FluentValidation;
using VehicleServiceBooking.Auth.Models.Requests;

namespace VehicleServiceBooking.Auth.Validators;

/// <summary>
/// Validates login request payloads.
/// </summary>
public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoginRequestValidator"/> class.
    /// </summary>
    public LoginRequestValidator()
    {
        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.Identifier) || !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("Either Identifier or Email must be provided.");

        RuleFor(x => x.Identifier)
            .MaximumLength(256);

        RuleFor(x => x.Email)
            .MaximumLength(256);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MaximumLength(256);
    }
}
