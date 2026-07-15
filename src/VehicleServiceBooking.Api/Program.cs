using System;
using System.IO;
using DotNetEnv;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using VehicleServiceBooking.Api.Configuration;
using VehicleServiceBooking.Observability;

//
// ============================================================================
// CONFIGURATION PRECEDENCE (Highest to Lowest Priority)
// ============================================================================
// 
// 1. Shell/host environment variables
// 2. .env fallback values only for missing keys (NoClobber)
// 3. appsettings.{Environment}.json
// 4. appsettings.json
//
// Usage:
//   Development:  dotnet run              (uses .env if it exists)
//   Staging:      ASPNETCORE_ENVIRONMENT=Staging dotnet run
//   Production:   ASPNETCORE_ENVIRONMENT=Production dotnet run
//
// ============================================================================
//

// Load .env for local fallback values only (NoClobber preserves shell/host vars)
// This must happen BEFORE CreateBuilder so environment variables are set
// before configuration is read
var envDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
while (envDirectory is not null)
{
    var envFile = Path.Combine(envDirectory.FullName, ".env");
    if (File.Exists(envFile))
    {
        Env.NoClobber().Load(envFile);
        break;
    }

    envDirectory = envDirectory.Parent;
}

var apiSpecificConnection = Environment.GetEnvironmentVariable("API__CONNECTIONSTRINGS__DEFAULTCONNECTION");
if (!string.IsNullOrWhiteSpace(apiSpecificConnection))
{
    Environment.SetEnvironmentVariable("CONNECTIONSTRINGS__DEFAULTCONNECTION", apiSpecificConnection);
}

var builder = WebApplication.CreateBuilder(args);

builder.AddLoggingAndTracing();

//
// ============================================================================
// DEPENDENCY INJECTION ORCHESTRATION
// ============================================================================
//
// The following extension methods register all services, configurations,
// and options in a clean, maintainable way. Each extension is responsible
// for a specific concern (controllers, swagger, CORS, database, services).
//
// This orchestrator pattern keeps Program.cs clean and allows each
// configuration concern to be modified independently.
//
// ============================================================================
//

// Register application options (Scheduling, CORS configuration)
builder.Services.AddApplicationOptions(builder.Configuration);

// Register controllers with validation
builder.Services.AddControllersWithValidation();

// Register Swagger/OpenAPI documentation
builder.Services.AddSwaggerDocumentation();

// Register persistence layer (DbContext)
builder.Services.AddPersistenceLayer(builder.Configuration);

// Register JWT authentication and authorization policies
builder.Services.AddJwtAuthentication(builder.Configuration);

// Register application services and repositories
builder.Services.AddApplicationServices();

// Build the application
var app = builder.Build();

//
// ============================================================================
// HTTP MIDDLEWARE PIPELINE ORCHESTRATION
// ============================================================================
//
// Middleware order is critical. They are executed in the order registered.
// Current order (from first to last):
//
// 1. Exception Handling - Catch and format exceptions
// 2. Swagger Documentation - API documentation UI
// 3. HTTPS Redirection - Enforce HTTPS
// 4. Authorization - Will be used for JWT in Phase 3
// 5. CORS - Must be after routing, before authorization in some cases
// 6. Controllers - Map to controller actions
//
// ============================================================================
//

// Configure middleware pipeline
app.UseSerilogRequestLogging(options =>
{
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        if (httpContext.Items.TryGetValue("CorrelationId", out var correlationId) && correlationId != null)
        {
            diagnosticContext.Set("CorrelationId", correlationId);
        }

        diagnosticContext.Set("TraceIdentifier", httpContext.TraceIdentifier);
    };
});

// Get CORS configuration and apply CORS middleware
var corsConfig = app.Services
    .GetRequiredService<VehicleServiceBooking.Application.Configuration.Interfaces.ICorsConfiguration>();

app.UseCorsPolicy(corsConfig.PolicyName);

app.UseApplicationMiddleware();

app.Run();
