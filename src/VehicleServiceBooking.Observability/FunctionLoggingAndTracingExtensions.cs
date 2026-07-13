using System;
using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace VehicleServiceBooking.Observability;

/// <summary>
/// Extension methods for configuring structured logging and tracing in Azure Functions isolated worker hosts.
/// </summary>
public static class FunctionLoggingAndTracingExtensions
{
    /// <summary>
    /// Configures Serilog and OpenTelemetry tracing for a generic host.
    /// </summary>
    public static IHostBuilder AddLoggingAndTracingForFunctions(this IHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.UseSerilog((context, _, loggerConfiguration) =>
        {
            var serilogSection = context.Configuration.GetSection("Observability:Serilog");
            var defaultLogLevel = ParseLogEventLevel(serilogSection.GetValue<string>("MinimumLevel") ?? "Information");
            var microsoftLogLevel = ParseLogEventLevel(
                serilogSection.GetValue<string>("Override:Microsoft") ?? "Information");
            var enableConsoleSink = serilogSection.GetValue<bool?>("EnableConsole") ?? true;
            var enableFileSink = serilogSection.GetValue<bool?>("EnableFile") ?? true;
            var filePath = serilogSection.GetValue<string>("FilePath") ?? "logs/notification-functions-.txt";
            var rollingIntervalValue = serilogSection.GetValue<string>("RollingInterval") ?? "Day";
            var rollingInterval = ParseRollingInterval(rollingIntervalValue);
            var outputTemplate = serilogSection.GetValue<string>("OutputTemplate")
                ?? "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}";

            loggerConfiguration
                .MinimumLevel.Override("Microsoft", microsoftLogLevel)
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
        });

        builder.ConfigureServices((context, services) =>
        {
            var tracingSection = context.Configuration.GetSection("Observability:OpenTelemetry:Tracing");
            var useHttpClientInstrumentation = tracingSection.GetValue<bool?>("UseHttpClientInstrumentation") ?? true;
            var exporter = tracingSection.GetValue<string>("Exporter") ?? "Console";
            var appInsightsConnectionString = ResolveApplicationInsightsConnectionString(context.Configuration, tracingSection);

            services
                .AddOpenTelemetry()
                .WithTracing(tracing =>
                {
                    if (useHttpClientInstrumentation)
                    {
                        tracing.AddHttpClientInstrumentation();
                    }

                    if (string.Equals(exporter, "Console", StringComparison.OrdinalIgnoreCase))
                    {
                        tracing.AddConsoleExporter();
                    }
                    else if (string.Equals(exporter, "AzureMonitor", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(exporter, "ApplicationInsights", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!string.IsNullOrWhiteSpace(appInsightsConnectionString))
                        {
                            tracing.AddAzureMonitorTraceExporter(options =>
                            {
                                options.ConnectionString = appInsightsConnectionString;
                            });
                        }
                    }
                });
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

    private static string? ResolveApplicationInsightsConnectionString(
        IConfiguration configuration,
        IConfigurationSection tracingSection)
    {
        var fromTracing = tracingSection.GetValue<string>("AzureMonitor:ConnectionString");
        if (!string.IsNullOrWhiteSpace(fromTracing))
        {
            return fromTracing;
        }

        var fromObservability = configuration.GetValue<string>("Observability:ApplicationInsights:ConnectionString");
        if (!string.IsNullOrWhiteSpace(fromObservability))
        {
            return fromObservability;
        }

        var fromEnvironment = Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING");
        return string.IsNullOrWhiteSpace(fromEnvironment) ? null : fromEnvironment;
    }
}
