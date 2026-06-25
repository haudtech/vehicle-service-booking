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
    /// Registers the database context with appropriate provider
    /// Currently uses InMemory for development; will be updated to SQL Server in Phase 3
    /// </summary>
    /// <param name="services">The dependency injection container</param>
    /// <param name="configuration">The application configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddPersistenceLayer(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // TODO: Phase 3 - Update to use actual database
        // Get connection string from configuration:
        // var connectionString = configuration.GetConnectionString("DefaultConnection")
        //     ?? throw new InvalidOperationException("Connection string not found: DefaultConnection");
        //
        // Then use:
        // services.AddDbContext<ApplicationDbContext>(options =>
        //     options.UseSqlServer(connectionString));

        // Phase 2: Use in-memory database for development
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseInMemoryDatabase("VehicleServiceBookingDb");
        });

        return services;
    }
}
