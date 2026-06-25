using System;
using System.IO;
using System.Reflection;
using DotNetEnv;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using VehicleServiceBooking.Application.Configuration;
using VehicleServiceBooking.Application.DTOs;
using VehicleServiceBooking.Application.Interfaces;
using VehicleServiceBooking.Application.Interfaces.Persistence;
using VehicleServiceBooking.Application.Interfaces.Repositories;
using VehicleServiceBooking.Application.Services;
using VehicleServiceBooking.Application.Validators;
using VehicleServiceBooking.Api.Middleware;
using VehicleServiceBooking.Infrastructure.Persistence;
using VehicleServiceBooking.Infrastructure.Repositories;

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

// At this point, CreateBuilder has automatically loaded:
// 1. appsettings.json
// 2. appsettings.{Environment}.json (based on ASPNETCORE_ENVIRONMENT)
// 3. Environment variables from OS/Docker/.env (highest priority)

//
// Controllers
//
builder.Services.AddControllers();

//
// FluentValidation
//
builder.Services.AddScoped<IValidator<GetAvailabilityRequest>, GetAvailabilityRequestValidator>();
builder.Services.AddScoped<IValidator<CreateAppointmentRequest>, CreateAppointmentRequestValidator>();

//
// Swagger
//
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Vehicle Service Booking API",
        Version = "v1.0.0",
        Description = "API for managing vehicle service appointments with real-time availability checking",
        Contact = new OpenApiContact
        {
            Name = "Vehicle Service Booking Support",
            Email = "support@vehicleservicebooking.com"
        },
        License = new OpenApiLicense
        {
            Name = "MIT License"
        }
    });

    // Add XML comments from controller and DTO classes
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    // Add authorization scheme (placeholder for future JWT implementation)
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert JWT token (future implementation)",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });

    // Group endpoints by tags
    options.TagActionsBy(api =>
    {
        if (api.GroupName != null)
        {
            return new[] { api.GroupName };
        }

        var controllerActionDescriptor = api.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor;
        if (controllerActionDescriptor != null)
        {
            return new[] { controllerActionDescriptor.ControllerName };
        }

        throw new InvalidOperationException("Unable to determine tag for endpoint.");
    });

    options.DocInclusionPredicate((name, api) => true);
});

//
// Scheduling Options (from appsettings.json)
//
builder.Services.Configure<SchedulingOptions>(
    builder.Configuration.GetSection(SchedulingOptions.SectionName));

builder.Services.AddSingleton<ISchedulingConfiguration>(sp =>
    sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<SchedulingOptions>>().Value);

//
// CORS (Cross-Origin Resource Sharing) Configuration
//
// Allows specified frontend applications to make requests to this API
// Configuration is environment-specific (see appsettings.{Environment}.json)
//
builder.Services.Configure<CorsOptions>(
    builder.Configuration.GetSection(CorsOptions.SectionName));

var corsConfig = builder.Configuration.GetSection(CorsOptions.SectionName).Get<CorsOptions>()
    ?? new CorsOptions();

// Add CORS service with the configured policy
builder.Services.AddCors(options =>
{
    options.AddPolicy(corsConfig.PolicyName, policy =>
    {
        // Configure allowed origins
        if (corsConfig.AllowedOrigins.Length > 0 && corsConfig.AllowedOrigins[0] == "*")
        {
            policy.AllowAnyOrigin();
        }
        else
        {
            policy.WithOrigins(corsConfig.AllowedOrigins);
        }

        // Configure allowed HTTP methods
        if (corsConfig.AllowedMethods.Length > 0)
        {
            policy.WithMethods(corsConfig.AllowedMethods);
        }
        else
        {
            policy.AllowAnyMethod();
        }

        // Configure allowed request headers
        if (corsConfig.AllowedHeaders.Length > 0 && corsConfig.AllowedHeaders[0] == "*")
        {
            policy.AllowAnyHeader();
        }
        else
        {
            policy.WithHeaders(corsConfig.AllowedHeaders);
        }

        // Configure exposed response headers
        if (corsConfig.ExposedHeaders.Length > 0)
        {
            policy.WithExposedHeaders(corsConfig.ExposedHeaders);
        }

        // Configure max age for preflight caching
        if (corsConfig.MaxAge > 0)
        {
            policy.SetPreflightMaxAge(TimeSpan.FromSeconds(corsConfig.MaxAge));
        }

        // Configure credentials
        if (corsConfig.AllowCredentials)
        {
            // Important: If AllowCredentials is true, AllowAnyOrigin() cannot be used
            // Must specify exact origins
            policy.AllowCredentials();
        }
    });
});

// Store CORS config as singleton for potential use in other services
builder.Services.AddSingleton<ICorsConfiguration>(corsConfig);

//
// DbContext (InMemory for now)
//
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseInMemoryDatabase("VehicleServiceBookingDb");
});

//
// Application DbContext abstraction
//
builder.Services.AddScoped<IApplicationDbContext>(sp =>
    sp.GetRequiredService<ApplicationDbContext>());

//
// Repository Pattern
//
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IServiceBayRepository, ServiceBayRepository>();
builder.Services.AddScoped<IAvailabilityRepository, AvailabilityRepository>();

//
// Application Services
//
builder.Services.AddScoped<IAvailabilityService, AvailabilityService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();

var app = builder.Build();

//
// HTTP pipeline - Middleware
//
app.UseMiddleware<ValidationExceptionMiddleware>();

//
// CORS Middleware - Must be placed after UseRouting but before UseAuthorization
// in some cases. Here it's placed early in the pipeline for all requests.
//
app.UseCors(corsConfig.PolicyName);

//
// HTTP pipeline - Standard middleware
//
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();