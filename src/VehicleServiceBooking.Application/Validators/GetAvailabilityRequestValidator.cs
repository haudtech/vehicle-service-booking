using System;
using FluentValidation;
using VehicleServiceBooking.Application.DTOs;

namespace VehicleServiceBooking.Application.Validators;

/// <summary>
/// Validator for availability query requests
/// </summary>
public class GetAvailabilityRequestValidator : AbstractValidator<GetAvailabilityRequest>
{
    public GetAvailabilityRequestValidator()
    {
        RuleFor(x => x.DealershipId)
            .NotEmpty()
            .WithMessage("Dealership ID cannot be empty")
            .WithErrorCode("INVALID_DEALERSHIP_ID");

        RuleFor(x => x.ServiceTypeId)
            .NotEmpty()
            .WithMessage("Service type ID cannot be empty")
            .WithErrorCode("INVALID_SERVICE_TYPE_ID");

        RuleFor(x => x.Date)
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
            .WithMessage("Date must be today or in the future")
            .WithErrorCode("INVALID_DATE");
    }
}
