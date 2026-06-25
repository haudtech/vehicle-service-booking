using System;
using FluentValidation;
using VehicleServiceBooking.Application.DTOs;

namespace VehicleServiceBooking.Application.Validators;

/// <summary>
/// Validator for appointment creation requests
/// </summary>
public class CreateAppointmentRequestValidator : AbstractValidator<CreateAppointmentRequest>
{
    public CreateAppointmentRequestValidator()
    {
        RuleFor(x => x.DealershipId)
            .NotEmpty()
            .WithMessage("Dealership ID cannot be empty")
            .WithErrorCode("INVALID_DEALERSHIP_ID");

        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID cannot be empty")
            .WithErrorCode("INVALID_CUSTOMER_ID");

        RuleFor(x => x.VehicleId)
            .NotEmpty()
            .WithMessage("Vehicle ID cannot be empty")
            .WithErrorCode("INVALID_VEHICLE_ID");

        RuleFor(x => x.ServiceTypeId)
            .NotEmpty()
            .WithMessage("Service type ID cannot be empty")
            .WithErrorCode("INVALID_SERVICE_TYPE_ID");

        RuleFor(x => x.TechnicianId)
            .NotEmpty()
            .WithMessage("Technician ID cannot be empty")
            .WithErrorCode("INVALID_TECHNICIAN_ID");

        RuleFor(x => x.ServiceBayId)
            .NotEmpty()
            .WithMessage("Service bay ID cannot be empty")
            .WithErrorCode("INVALID_SERVICE_BAY_ID");

        RuleFor(x => x.SlotStart)
            .GreaterThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Slot start time must be in the future")
            .WithErrorCode("INVALID_SLOT_START");

        RuleFor(x => x.SlotEnd)
            .GreaterThan(x => x.SlotStart)
            .WithMessage("Slot end time must be after start time")
            .WithErrorCode("INVALID_SLOT_END");
    }
}
