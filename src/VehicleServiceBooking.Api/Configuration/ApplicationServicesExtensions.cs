using Microsoft.Extensions.DependencyInjection;
using VehicleServiceBooking.Application.Configuration.Interfaces;
using VehicleServiceBooking.Application.Interfaces;
using VehicleServiceBooking.Application.Interfaces.Persistence;
using VehicleServiceBooking.Application.Interfaces.Repositories;
using VehicleServiceBooking.Application.Interfaces.Services;
using VehicleServiceBooking.Application.Services;
using VehicleServiceBooking.Infrastructure.Persistence;
using VehicleServiceBooking.Infrastructure.Repositories;

namespace VehicleServiceBooking.Api.Configuration;

/// <summary>
/// Extension methods for configuring application services and repositories
/// Orchestrates all business logic layer registrations
/// </summary>
public static class ApplicationServicesExtensions
{
    /// <summary>
    /// Registers all application services and repositories
    /// </summary>
    /// <param name="services">The dependency injection container</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
        // Register DbContext abstraction
        services.AddScoped<IApplicationDbContext>(sp =>
            sp.GetRequiredService<ApplicationDbContext>());

        // Register Repositories
        RegisterRepositories(services);

        // Register Application Services
        RegisterApplicationServices(services);

        return services;
    }

    /// <summary>
    /// Registers all repository implementations
    /// Follows Repository Pattern for data access abstraction
    /// </summary>
    private static void RegisterRepositories(IServiceCollection services)
    {
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<IServiceBayRepository, ServiceBayRepository>();
        services.AddScoped<IAvailabilityRepository, AvailabilityRepository>();
    }

    /// <summary>
    /// Registers all application service implementations
    /// Services contain business logic and use repositories for data access
    /// </summary>
    private static void RegisterApplicationServices(IServiceCollection services)
    {
        services.AddScoped<IAvailabilityService, AvailabilityService>();
        services.AddScoped<IAppointmentService, AppointmentService>();
    }
}
