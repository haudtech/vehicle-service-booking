using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace VehicleServiceBooking.Api.Configuration;

/// <summary>
/// Extension methods for configuring structured logging and tracing.
/// </summary>
public static class LogConfigurationExtensions
{
    /// <summary>
    /// Configures Serilog and OpenTelemetry tracing from configuration.
    /// </summary>
    public static WebApplicationBuilder AddLoggingAndTracing(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var serilogSection = builder.Configuration.GetSection("Observability:Serilog");
        var defaultLogLevel = ParseLogEventLevel(serilogSection.GetValue<string>("MinimumLevel") ?? "Information");
        var microsoftLogLevel = ParseLogEventLevel(
            serilogSection.GetValue<string>("Override:Microsoft") ?? "Information");
        var aspNetCoreLogLevel = ParseLogEventLevel(
            serilogSection.GetValue<string>("Override:Microsoft.AspNetCore") ?? "Warning");
        var enableConsoleSink = serilogSection.GetValue<bool?>("EnableConsole") ?? true;
        var enableFileSink = serilogSection.GetValue<bool?>("EnableFile") ?? true;
        var filePath = serilogSection.GetValue<string>("FilePath") ?? "logs/app-.txt";
        var rollingIntervalValue = serilogSection.GetValue<string>("RollingInterval") ?? "Day";
        var rollingInterval = ParseRollingInterval(rollingIntervalValue);
        var outputTemplate = serilogSection.GetValue<string>("OutputTemplate")
            ?? "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}";

        var loggerConfiguration = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", microsoftLogLevel)
            .MinimumLevel.Override("Microsoft.AspNetCore", aspNetCoreLogLevel)
            .MinimumLevel.Is(defaultLogLevel)
            .Enrich.FromLogContext();

        if (enableConsoleSink)
        {
            loggerConfiguration.WriteTo.Console(outputTemplate: outputTemplate, theme: AnsiConsoleTheme.Code);
        }

        if (enableFileSink)
        {
            loggerConfiguration.WriteTo.File(filePath, rollingInterval: rollingInterval, outputTemplate: outputTemplate);
        }

        Log.Logger = loggerConfiguration.CreateLogger();
        builder.Host.UseSerilog();

        var tracingSection = builder.Configuration.GetSection("Observability:OpenTelemetry:Tracing");
        var useAspNetCoreInstrumentation = tracingSection.GetValue<bool?>("UseAspNetCoreInstrumentation") ?? true;
        var useEntityFrameworkCoreInstrumentation = tracingSection.GetValue<bool?>("UseEntityFrameworkCoreInstrumentation") ?? true;
        var exporter = tracingSection.GetValue<string>("Exporter") ?? "Console";

        builder.Services
            .AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                if (useAspNetCoreInstrumentation)
                {
                    tracing.AddAspNetCoreInstrumentation();
                }

                if (useEntityFrameworkCoreInstrumentation)
                {
                    tracing.AddEntityFrameworkCoreInstrumentation();
                }

                if (string.Equals(exporter, "Console", StringComparison.OrdinalIgnoreCase))
                {
                    tracing.AddConsoleExporter();
                }
            });

        return builder;
    }

    private static LogEventLevel ParseLogEventLevel(string value)
    {
        return Enum.TryParse<LogEventLevel>(value, ignoreCase: true, out var level)
            ? level
            : LogEventLevel.Information;
    }

    private static RollingInterval ParseRollingInterval(string value)
    {
        return Enum.TryParse<RollingInterval>(value, ignoreCase: true, out var interval)
            ? interval
            : RollingInterval.Day;
    }
}
