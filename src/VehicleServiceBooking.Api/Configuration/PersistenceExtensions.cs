using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VehicleServiceBooking.Infrastructure.Persistence;

namespace VehicleServiceBooking.Api.Configuration;

/// <summary>
/// Extension methods for configuring database and persistence layer
/// </summary>
public static class PersistenceExtensions
{
    /// <summary>
    /// Registers the database context with PostgreSQL provider
    /// Supports development, staging, and production environments
    /// Connection strings are configured in appsettings.{Environment}.json
    /// </summary>
    /// <param name="services">The dependency injection container</param>
    /// <param name="configuration">The application configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddPersistenceLayer(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Get connection string from configuration
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' not found in appsettings.json. " +
                "Please ensure you have configured the connection string in appsettings.{Environment}.json");

        // Register ApplicationDbContext with PostgreSQL provider
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(
                connectionString,
                npgsqlOptions =>
                {
                    // Enable migrations assembly
                    npgsqlOptions.MigrationsAssembly("VehicleServiceBooking.Infrastructure");
                    // Enable retry on transient failures
                    npgsqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(10), null);
                });
        });

        return services;
    }
}
