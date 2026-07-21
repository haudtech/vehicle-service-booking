using System;
using FluentValidation;
using VehicleServiceBooking.Application.DTOs;
using VehicleServiceBooking.Domain.Enums;

namespace VehicleServiceBooking.Application.Validators;

/// <summary>
/// Validator for inbound payment webhook request payload.
/// </summary>
public sealed class ProcessPaymentWebhookRequestValidator : AbstractValidator<ProcessPaymentWebhookRequest>
{
    public ProcessPaymentWebhookRequestValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID cannot be empty")
            .WithErrorCode("INVALID_EVENT_ID");

        RuleFor(x => x.IntentCode)
            .NotEmpty()
            .WithMessage("Intent code cannot be empty")
            .WithErrorCode("INVALID_INTENT_CODE");

        RuleFor(x => x.TransactionStatus)
            .NotEmpty()
            .WithMessage("Transaction status cannot be empty")
            .WithErrorCode("INVALID_TRANSACTION_STATUS")
            .Must(BeValidTransactionStatus)
            .WithMessage("Transaction status is not supported")
            .WithErrorCode("UNSUPPORTED_TRANSACTION_STATUS");

        RuleFor(x => x.SignatureHash)
            .NotEmpty()
            .WithMessage("Signature hash cannot be empty")
            .WithErrorCode("INVALID_SIGNATURE_HASH");
    }

    private static bool BeValidTransactionStatus(string transactionStatus)
    {
        return Enum.TryParse<PaymentTransactionStatus>(transactionStatus, true, out _);
    }
}