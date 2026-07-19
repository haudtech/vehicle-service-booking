using System;
using System.Linq;
using FluentValidation;
using VehicleServiceBooking.Application.DTOs;

namespace VehicleServiceBooking.Application.Validators;

/// <summary>
/// Validator for order creation requests.
/// </summary>
public sealed class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.OrderCode)
            .NotEmpty()
            .WithMessage("Order code cannot be empty")
            .WithErrorCode("INVALID_ORDER_CODE")
            .MaximumLength(50)
            .WithMessage("Order code cannot exceed 50 characters")
            .WithErrorCode("INVALID_ORDER_CODE");

        RuleFor(x => x.CurrencyId)
            .NotEmpty()
            .WithMessage("Currency ID cannot be empty")
            .WithErrorCode("INVALID_CURRENCY_ID");

        RuleFor(x => x.ServiceTypeItems)
            .NotNull()
            .WithMessage("Service type items are required")
            .WithErrorCode("INVALID_SERVICE_TYPE_ITEMS")
            .Must(x => x.Count > 0)
            .WithMessage("At least one service type item is required")
            .WithErrorCode("INVALID_SERVICE_TYPE_ITEMS");

        RuleForEach(x => x.ServiceTypeItems)
            .ChildRules(item =>
            {
                item.RuleFor(i => i.ServiceTypeId)
                    .NotEmpty()
                    .WithMessage("Service type ID cannot be empty")
                    .WithErrorCode("INVALID_SERVICE_TYPE_ID");

                item.RuleFor(i => i.Quantity)
                    .GreaterThan(0)
                    .WithMessage("Quantity must be greater than 0")
                    .WithErrorCode("INVALID_QUANTITY");
            });

        RuleFor(x => x)
            .Custom((request, context) =>
            {
                if (request.ServiceTypeItems is null || request.ServiceTypeItems.Count == 0)
                {
                    return;
                }

                var duplicateServiceTypeIds = request.ServiceTypeItems
                    .GroupBy(x => x.ServiceTypeId)
                    .Where(g => g.Key != Guid.Empty && g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();

                if (duplicateServiceTypeIds.Count > 0)
                {
                    context.AddFailure(new FluentValidation.Results.ValidationFailure(
                        nameof(CreateOrderRequest.ServiceTypeItems),
                        "ServiceTypeItems must not contain duplicate ServiceTypeId values")
                    {
                        ErrorCode = "DUPLICATE_SERVICE_TYPE"
                    });
                }

                if (request.AppointmentIds is null || request.AppointmentIds.Count == 0)
                {
                    return;
                }

                if (request.AppointmentIds.Any(id => id == Guid.Empty))
                {
                    context.AddFailure(new FluentValidation.Results.ValidationFailure(
                        nameof(CreateOrderRequest.AppointmentIds),
                        "AppointmentIds cannot contain empty GUID values")
                    {
                        ErrorCode = "INVALID_APPOINTMENT_ID"
                    });
                }

                var duplicateAppointmentIds = request.AppointmentIds
                    .GroupBy(x => x)
                    .Where(g => g.Key != Guid.Empty && g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();

                if (duplicateAppointmentIds.Count > 0)
                {
                    context.AddFailure(new FluentValidation.Results.ValidationFailure(
                        nameof(CreateOrderRequest.AppointmentIds),
                        "AppointmentIds must not contain duplicate values")
                    {
                        ErrorCode = "DUPLICATE_APPOINTMENT_ID"
                    });
                }
            });
    }
}