using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VehicleServiceBooking.Application.Configuration;
using VehicleServiceBooking.Application.Configuration.Interfaces;

namespace VehicleServiceBooking.Api.Configuration;

/// <summary>
/// Extension methods for configuring application options and settings
/// This orchestrates all configuration registrations in one place
/// </summary>
public static class ApplicationOptionsExtensions
{
    /// <summary>
    /// Registers all application configuration options (scheduling, CORS, etc.)
    /// These options are bound from appsettings.json and environment variables
    /// </summary>
    /// <param name="services">The dependency injection container</param>
    /// <param name="configuration">The application configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddApplicationOptions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register SchedulingOptions
        services.Configure<SchedulingOptions>(
            configuration.GetSection(SchedulingOptions.SectionName));

        services.AddSingleton<ISchedulingConfiguration>(sp =>
            sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<SchedulingOptions>>().Value);

        // Register CorsOptions
        services.Configure<CorsOptions>(
            configuration.GetSection(CorsOptions.SectionName));

        var corsConfig = configuration.GetSection(CorsOptions.SectionName).Get<CorsOptions>()
            ?? new CorsOptions();

        services.AddSingleton<ICorsConfiguration>(corsConfig);

        // Register static data cache options
        services.Configure<StaticDataCacheOptions>(
            configuration.GetSection(StaticDataCacheOptions.SectionName));

        // Register idempotency options
        services.Configure<IdempotencyOptions>(
            configuration.GetSection(IdempotencyOptions.SectionName));

        services
            .AddOptions<AuthUserProfileOptions>()
            .Bind(configuration.GetSection(AuthUserProfileOptions.SectionName))
            .ValidateDataAnnotations()
            .Validate(
                HasValidAuthUserProfileEndpoint,
                $"{AuthUserProfileOptions.SectionName} must configure either a valid absolute BaseUrl or Host (with optional Scheme/Port/BasePath).")
            .Validate(
                options => (options.CoreProfilePathTemplate ?? string.Empty).Contains("{authUserId}", StringComparison.OrdinalIgnoreCase),
                $"{AuthUserProfileOptions.SectionName}:CoreProfilePathTemplate must contain {{authUserId}} placeholder.")
            .ValidateOnStart();

        // Register in-process cache provider
        services.AddMemoryCache();

        return services;
    }

    private static bool HasValidAuthUserProfileEndpoint(AuthUserProfileOptions options)
    {
        if (!string.IsNullOrWhiteSpace(options.BaseUrl))
        {
            return Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out _);
        }

        if (string.IsNullOrWhiteSpace(options.Host))
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(options.Scheme) && !Uri.CheckSchemeName(options.Scheme))
        {
            return false;
        }

        return !options.Port.HasValue || (options.Port.Value >= 1 && options.Port.Value <= 65535);
    }
}
