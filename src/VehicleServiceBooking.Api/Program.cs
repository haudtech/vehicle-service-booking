using System;
using System.IO;
using DotNetEnv;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VehicleServiceBooking.Api.Configuration;

//
// ============================================================================
// CONFIGURATION PRECEDENCE (Lowest to Highest Priority)
// ============================================================================
// 
// 1. appsettings.json              - Base configuration (always loaded)
// 2. appsettings.{Environment}.json - Environment overrides
//    - Development  → appsettings.Development.json
//    - Staging      → appsettings.Staging.json
//    - Production   → appsettings.Production.json
// 3. .env file                     - Local development overrides (highest)
//    - Loaded by DotNetEnv before CreateBuilder
//    - Converted to environment variables by the OS
//    - Environment variables override appsettings files
//
// Usage:
//   Development:  dotnet run              (uses .env if it exists)
//   Staging:      ASPNETCORE_ENVIRONMENT=Staging dotnet run
//   Production:   ASPNETCORE_ENVIRONMENT=Production dotnet run
//
// ============================================================================
//

// Load environment variables from .env file (for local development)
// This must happen BEFORE CreateBuilder so environment variables are set
// before configuration is read
var envFile = Path.Combine(Directory.GetCurrentDirectory(), ".env");
if (File.Exists(envFile))
{
    Env.Load(envFile);
}

var builder = WebApplication.CreateBuilder(args);

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
app.UseApplicationMiddleware();

// Get CORS configuration and apply CORS middleware
var corsConfig = builder.Services.BuildServiceProvider()
    .GetRequiredService<VehicleServiceBooking.Application.Configuration.Interfaces.ICorsConfiguration>();

app.UseCorsPolicy(corsConfig.PolicyName);

app.Run();