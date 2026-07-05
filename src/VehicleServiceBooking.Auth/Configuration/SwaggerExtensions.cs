using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace VehicleServiceBooking.Auth.Configuration;

/// <summary>
/// Extension methods for configuring Swagger/OpenAPI documentation.
/// </summary>
public static class SwaggerExtensions
{
    /// <summary>
    /// Registers Swagger/OpenAPI services.
    /// </summary>
    public static IServiceCollection AddSwaggerDocumentation(
        this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Vehicle Service Booking Auth API",
                Version = "v1.0.0",
                Description = "Authentication and authorization API for Vehicle Service Booking"
            });
        });

        return services;
    }
}