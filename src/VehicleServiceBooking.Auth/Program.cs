using System.IO;
using DotNetEnv;
using VehicleServiceBooking.Auth.Configuration;
using Serilog;

var envFile = Path.Combine(Directory.GetCurrentDirectory(), ".env");
if (File.Exists(envFile))
{
	Env.Load(envFile);
}

var builder = WebApplication.CreateBuilder(args);

builder.AddLoggingAndTracing();

// Register controllers and API behavior.
builder.Services.AddControllersWithValidation();

// Register Swagger/OpenAPI documentation.
builder.Services.AddSwaggerDocumentation();

// Register auth service dependencies (DbContext, authentication, services).
builder.Services.AddAuthServiceDependencies(builder.Configuration);

var app = builder.Build();

app.UseSerilogRequestLogging();

// Configure middleware pipeline.
app.UseApplicationMiddleware();

app.Run();
