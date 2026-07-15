using FluentValidation;
using VehicleServiceBooking.Auth.Models.Requests;

namespace VehicleServiceBooking.Auth.Validators;

/// <summary>
/// Validates verify-email request payloads.
/// </summary>
public sealed class VerifyEmailRequestValidator : AbstractValidator<VerifyEmailRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VerifyEmailRequestValidator"/> class.
    /// </summary>
    public VerifyEmailRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(x => x.Token)
            .NotEmpty()
            .MinimumLength(16)
            .MaximumLength(512);
    }
}