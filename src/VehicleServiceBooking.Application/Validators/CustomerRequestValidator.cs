using FluentValidation;
using VehicleServiceBooking.Application.DTOs;

namespace VehicleServiceBooking.Application.Validators;

/// <summary>
/// Validator for customer-related requests
/// Rules align with database constraints in Customer entity
/// </summary>
public class CreateCustomerRequestValidator : AbstractValidator<CreateCustomerRequest>
{
    public CreateCustomerRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required")
            .WithErrorCode("FIRST_NAME_REQUIRED")
            .MaximumLength(100)
            .WithMessage("First name cannot exceed 100 characters")
            .WithErrorCode("FIRST_NAME_TOO_LONG");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required")
            .WithErrorCode("LAST_NAME_REQUIRED")
            .MaximumLength(100)
            .WithMessage("Last name cannot exceed 100 characters")
            .WithErrorCode("LAST_NAME_TOO_LONG");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .WithErrorCode("EMAIL_REQUIRED")
            .EmailAddress()
            .WithMessage("Invalid email format")
            .WithErrorCode("INVALID_EMAIL_FORMAT")
            .MaximumLength(254)
            .WithMessage("Email cannot exceed 254 characters (RFC 5321)")
            .WithErrorCode("EMAIL_TOO_LONG");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("Phone number is required")
            .WithErrorCode("PHONE_REQUIRED")
            .MaximumLength(20)
            .WithMessage("Phone number cannot exceed 20 characters")
            .WithErrorCode("PHONE_TOO_LONG")
            .Matches(@"^\+?1?\d{9,15}$")
            .WithMessage("Invalid phone number format (e.g., +1-234-567-8900 or 2345678900)")
            .WithErrorCode("INVALID_PHONE_FORMAT");
    }
}

/// <summary>
/// DTO for creating a new customer
/// Maps to Customer entity with validated constraints
/// </summary>
public class CreateCustomerRequest
{
    /// <summary>
    /// Customer's first name (max 100 chars)
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Customer's last name (max 100 chars)
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Customer's email address (max 254 chars, RFC 5321 standard)
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Customer's phone number (max 20 chars, supports international format)
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;
}
