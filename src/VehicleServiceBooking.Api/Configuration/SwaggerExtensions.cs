using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace VehicleServiceBooking.Api.Configuration;

/// <summary>
/// Extension methods for configuring Swagger/OpenAPI documentation
/// </summary>
public static class SwaggerExtensions
{
    /// <summary>
    /// Registers Swagger/OpenAPI services with comprehensive documentation
    /// </summary>
    /// <param name="services">The dependency injection container</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddSwaggerDocumentation(
        this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            // Add API info
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Vehicle Service Booking API",
                Version = "v1.0.0",
                Description = "API for managing vehicle service appointments with real-time availability checking",
                Contact = new OpenApiContact
                {
                    Name = "Vehicle Service Booking Support",
                    Email = "support@vehicleservicebooking.com"
                },
                License = new OpenApiLicense
                {
                    Name = "MIT License"
                }
            });

            // Add XML documentation
            AddXmlDocumentation(options);

            // Add security definition for JWT (future implementation)
            AddJwtSecurityDefinition(options);

            // Configure endpoint grouping
            ConfigureEndpointGrouping(options);
        });

        return services;
    }

    /// <summary>
    /// Adds XML documentation comments from assembly
    /// </summary>
    private static void AddXmlDocumentation(Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions options)
    {
        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath);
        }
    }

    /// <summary>
    /// Adds JWT authentication scheme definition (for Phase 3 implementation)
    /// </summary>
    private static void AddJwtSecurityDefinition(Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions options)
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please insert JWT token (future implementation in Phase 3)",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "bearer"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] { }
            }
        });
    }

    /// <summary>
    /// Configures endpoint grouping and filtering for Swagger UI
    /// </summary>
    private static void ConfigureEndpointGrouping(Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions options)
    {
        options.TagActionsBy(api =>
        {
            if (api.GroupName != null)
            {
                return new[] { api.GroupName };
            }

            var controllerActionDescriptor = api.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor;
            if (controllerActionDescriptor != null)
            {
                return new[] { controllerActionDescriptor.ControllerName };
            }

            throw new InvalidOperationException("Unable to determine tag for endpoint.");
        });

        options.DocInclusionPredicate((name, api) => true);
    }
}
