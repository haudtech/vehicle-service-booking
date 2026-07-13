using System.IO;
using DotNetEnv;
using VehicleServiceBooking.Auth.Configuration;
using Serilog;

var envDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
while (envDirectory is not null)
{
	var envFile = Path.Combine(envDirectory.FullName, ".env");
	if (File.Exists(envFile))
	{
		Env.Load(envFile);
		break;
	}

	envDirectory = envDirectory.Parent;
}

var authSpecificConnection = Environment.GetEnvironmentVariable("AUTH__CONNECTIONSTRINGS__DEFAULTCONNECTION");
if (!string.IsNullOrWhiteSpace(authSpecificConnection))
{
	Environment.SetEnvironmentVariable("CONNECTIONSTRINGS__DEFAULTCONNECTION", authSpecificConnection);
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
