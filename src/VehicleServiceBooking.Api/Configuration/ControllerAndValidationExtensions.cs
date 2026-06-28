using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using VehicleServiceBooking.Application.DTOs;
using VehicleServiceBooking.Application.Validators;

namespace VehicleServiceBooking.Api.Configuration;

/// <summary>
/// Extension methods for configuring controllers, validation, and API behavior
/// </summary>
public static class ControllerAndValidationExtensions
{
    /// <summary>
    /// Registers controllers and FluentValidation validators
    /// </summary>
    /// <param name="services">The dependency injection container</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddControllersWithValidation(
        this IServiceCollection services)
    {
        // Add Controllers
        services.AddControllers(options =>
        {
            // Add global model validation filter if needed
            options.Filters.Add<ValidationExceptionFilter>();
        });

        // Register FluentValidation validators
        services.AddScoped<IValidator<GetAvailabilityRequest>, GetAvailabilityRequestValidator>();
        services.AddScoped<IValidator<CreateAppointmentRequest>, CreateAppointmentRequestValidator>();

        // Configure API behavior
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = false;
        });

        return services;
    }
}

/// <summary>
/// Placeholder for validation exception filter (used by controller configuration)
/// </summary>
public class ValidationExceptionFilter : Microsoft.AspNetCore.Mvc.Filters.IActionFilter
{
    public void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context) { }
    public void OnActionExecuted(Microsoft.AspNetCore.Mvc.Filters.ActionExecutedContext context) { }
}
