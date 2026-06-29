using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

        // Register in-process cache provider
        services.AddMemoryCache();

        return services;
    }
}
