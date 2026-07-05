using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using VehicleServiceBooking.Auth.Models.Requests;
using VehicleServiceBooking.Auth.Validators;

namespace VehicleServiceBooking.Auth.Configuration;

/// <summary>
/// Extension methods for configuring controllers and API behavior.
/// </summary>
public static class ControllerAndValidationExtensions
{
    /// <summary>
    /// Registers controllers and default API behavior for model validation.
    /// </summary>
    public static IServiceCollection AddControllersWithValidation(
        this IServiceCollection services)
    {
        services.AddControllers();

        services.AddScoped<IValidator<SignUpRequest>, SignUpRequestValidator>();
        services.AddScoped<IValidator<LoginRequest>, LoginRequestValidator>();
        services.AddScoped<IValidator<RefreshTokenRequest>, RefreshTokenRequestValidator>();

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = false;
        });

        return services;
    }
}