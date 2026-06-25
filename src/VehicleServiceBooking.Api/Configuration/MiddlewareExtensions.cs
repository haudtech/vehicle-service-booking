using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using VehicleServiceBooking.Api.Middleware;

namespace VehicleServiceBooking.Api.Configuration;

/// <summary>
/// Extension methods for configuring HTTP middleware pipeline
/// </summary>
public static class MiddlewareExtensions
{
    /// <summary>
    /// Configures the application middleware pipeline
    /// Order matters: middleware is executed in the order registered
    /// </summary>
    /// <param name="app">The application builder</param>
    /// <returns>The application builder for chaining</returns>
    public static WebApplication UseApplicationMiddleware(
        this WebApplication app)
    {
        // Exception handling - should be first in pipeline
        app.UseMiddleware<ValidationExceptionMiddleware>();

        // Swagger documentation - only in Development
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // HTTPS redirection
        app.UseHttpsRedirection();

        // Authorization (will be used in Phase 3 for JWT)
        app.UseAuthorization();

        // Map controllers
        app.MapControllers();

        return app;
    }

    /// <summary>
    /// Configures CORS middleware to the pipeline
    /// Should be applied before authorization but after routing
    /// </summary>
    /// <param name="app">The application builder</param>
    /// <param name="corsPolicyName">The CORS policy name to use</param>
    /// <returns>The application builder for chaining</returns>
    public static WebApplication UseCorsPolicy(
        this WebApplication app,
        string corsPolicyName)
    {
        app.UseCors(corsPolicyName);
        return app;
    }
}
