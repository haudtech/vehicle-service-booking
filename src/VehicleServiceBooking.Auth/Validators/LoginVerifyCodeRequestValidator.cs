using FluentValidation;
using VehicleServiceBooking.Auth.Models.Requests;

namespace VehicleServiceBooking.Auth.Validators;

/// <summary>
/// Validates login-code verification request payloads.
/// </summary>
public sealed class LoginVerifyCodeRequestValidator : AbstractValidator<LoginVerifyCodeRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoginVerifyCodeRequestValidator"/> class.
    /// </summary>
    public LoginVerifyCodeRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(x => x.ChallengeId)
            .NotEmpty();

        RuleFor(x => x.Code)
            .NotEmpty()
            .MinimumLength(4)
            .MaximumLength(16);
    }
}
