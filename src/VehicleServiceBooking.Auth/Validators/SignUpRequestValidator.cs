using FluentValidation;
using VehicleServiceBooking.Auth.Models.Requests;

namespace VehicleServiceBooking.Auth.Validators;

/// <summary>
/// Validates sign-up request payloads.
/// </summary>
public sealed class SignUpRequestValidator : AbstractValidator<SignUpRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SignUpRequestValidator"/> class.
    /// </summary>
    public SignUpRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(x => x.AccountName)
            .NotEmpty()
            .MaximumLength(50)
            .Matches("^[a-zA-Z0-9._-]+$")
            .WithMessage("AccountName may only contain letters, numbers, dot, underscore, and hyphen.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(256);

        RuleFor(x => x.DisplayName)
            .NotEmpty()
            .MaximumLength(100);
    }
}
