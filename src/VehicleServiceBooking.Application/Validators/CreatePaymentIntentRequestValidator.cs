using FluentValidation;
using VehicleServiceBooking.Application.DTOs;

namespace VehicleServiceBooking.Application.Validators;

/// <summary>
/// Validator for payment intent creation request payload.
/// </summary>
public sealed class CreatePaymentIntentRequestValidator : AbstractValidator<CreatePaymentIntentRequest>
{
    public CreatePaymentIntentRequestValidator()
    {
        RuleFor(x => x.PaymentProviderId)
            .NotEmpty()
            .WithMessage("Payment provider ID cannot be empty")
            .WithErrorCode("INVALID_PAYMENT_PROVIDER_ID");

        RuleFor(x => x.PaymentMethodId)
            .NotEmpty()
            .WithMessage("Payment method ID cannot be empty")
            .WithErrorCode("INVALID_PAYMENT_METHOD_ID");
    }
}
