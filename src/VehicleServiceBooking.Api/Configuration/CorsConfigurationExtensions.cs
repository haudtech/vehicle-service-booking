using System;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using VehicleServiceBooking.Application.Configuration.Interfaces;

namespace VehicleServiceBooking.Api.Configuration;

/// <summary>
/// Extension methods for configuring CORS (Cross-Origin Resource Sharing)
/// </summary>
public static class CorsConfigurationExtensions
{
    /// <summary>
    /// Registers and configures CORS services with environment-specific settings
    /// </summary>
    /// <param name="services">The dependency injection container</param>
    /// <param name="corsConfiguration">The CORS configuration from options</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddCorsConfiguration(
        this IServiceCollection services,
        ICorsConfiguration corsConfiguration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(corsConfiguration.PolicyName, policy =>
            {
                ConfigureOrigins(policy, corsConfiguration);
                ConfigureMethods(policy, corsConfiguration);
                ConfigureHeaders(policy, corsConfiguration);
                ConfigureExposedHeaders(policy, corsConfiguration);
                ConfigureMaxAge(policy, corsConfiguration);
                ConfigureCredentials(policy, corsConfiguration);
            });
        });

        return services;
    }

    /// <summary>
    /// Configures allowed origins for CORS requests
    /// </summary>
    private static void ConfigureOrigins(
        Microsoft.AspNetCore.Cors.Infrastructure.CorsPolicyBuilder policy,
        ICorsConfiguration corsConfiguration)
    {
        if (corsConfiguration.AllowedOrigins.Length > 0 && corsConfiguration.AllowedOrigins[0] == "*")
        {
            policy.AllowAnyOrigin();
        }
        else if (corsConfiguration.AllowedOrigins.Length > 0)
        {
            policy.WithOrigins(corsConfiguration.AllowedOrigins);
        }
    }

    /// <summary>
    /// Configures allowed HTTP methods for CORS requests
    /// </summary>
    private static void ConfigureMethods(
        Microsoft.AspNetCore.Cors.Infrastructure.CorsPolicyBuilder policy,
        ICorsConfiguration corsConfiguration)
    {
        if (corsConfiguration.AllowedMethods.Length > 0)
        {
            policy.WithMethods(corsConfiguration.AllowedMethods);
        }
        else
        {
            policy.AllowAnyMethod();
        }
    }

    /// <summary>
    /// Configures allowed request headers for CORS requests
    /// </summary>
    private static void ConfigureHeaders(
        Microsoft.AspNetCore.Cors.Infrastructure.CorsPolicyBuilder policy,
        ICorsConfiguration corsConfiguration)
    {
        if (corsConfiguration.AllowedHeaders.Length > 0 && corsConfiguration.AllowedHeaders[0] == "*")
        {
            policy.AllowAnyHeader();
        }
        else if (corsConfiguration.AllowedHeaders.Length > 0)
        {
            policy.WithHeaders(corsConfiguration.AllowedHeaders);
        }
    }

    /// <summary>
    /// Configures exposed response headers that browsers are allowed to access
    /// </summary>
    private static void ConfigureExposedHeaders(
        Microsoft.AspNetCore.Cors.Infrastructure.CorsPolicyBuilder policy,
        ICorsConfiguration corsConfiguration)
    {
        if (corsConfiguration.ExposedHeaders.Length > 0)
        {
            policy.WithExposedHeaders(corsConfiguration.ExposedHeaders);
        }
    }

    /// <summary>
    /// Configures the maximum age for preflight cache
    /// </summary>
    private static void ConfigureMaxAge(
        Microsoft.AspNetCore.Cors.Infrastructure.CorsPolicyBuilder policy,
        ICorsConfiguration corsConfiguration)
    {
        if (corsConfiguration.MaxAge > 0)
        {
            policy.SetPreflightMaxAge(TimeSpan.FromSeconds(corsConfiguration.MaxAge));
        }
    }

    /// <summary>
    /// Configures whether credentials are allowed
    /// Important: If credentials are allowed, wildcard origins cannot be used
    /// </summary>
    private static void ConfigureCredentials(
        Microsoft.AspNetCore.Cors.Infrastructure.CorsPolicyBuilder policy,
        ICorsConfiguration corsConfiguration)
    {
        if (corsConfiguration.AllowCredentials)
        {
            policy.AllowCredentials();
        }
    }
}
