using Microsoft.AspNetCore.Builder;
using VehicleServiceBooking.Auth.Middleware;

namespace VehicleServiceBooking.Auth.Configuration;

/// <summary>
/// Extension methods for configuring HTTP middleware pipeline.
/// </summary>
public static class MiddlewareExtensions
{
    /// <summary>
    /// Configures middleware pipeline in a single orchestrator call.
    /// </summary>
    public static WebApplication UseApplicationMiddleware(
        this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseMiddleware<RequestValidationMiddleware>();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        return app;
    }
}