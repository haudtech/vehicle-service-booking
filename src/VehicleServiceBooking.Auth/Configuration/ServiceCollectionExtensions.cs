using Azure.Storage.Queues;
using System.IO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using VehicleServiceBooking.Auth.Data;
using VehicleServiceBooking.Auth.Repositories;
using VehicleServiceBooking.Auth.Repositories.Interfaces;
using VehicleServiceBooking.Auth.Services;

namespace VehicleServiceBooking.Auth.Configuration;

/// <summary>
/// Provides dependency injection registration helpers for the auth service.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers auth options, persistence, repositories, domain services, and JWT authentication.
    /// </summary>
    /// <param name="services">The service collection to populate.</param>
    /// <param name="configuration">Application configuration.</param>
    /// <returns>The updated service collection.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when required JWT settings or database connection string are missing.
    /// </exception>
    public static IServiceCollection AddAuthServiceDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
        services.Configure<KeyRotationOptions>(configuration.GetSection("KeyRotation"));
        services.Configure<DataProtectionKeyManagementOptions>(configuration.GetSection("DataProtection:KeyManagement"));
        services.Configure<GoogleAuthOptions>(configuration.GetSection("Authentication:Google"));
        services.Configure<NotificationOptions>(configuration.GetSection("Notification"));

        var jwtOptions = new JwtOptions();
        configuration.GetSection("Jwt").Bind(jwtOptions);
        if (string.IsNullOrWhiteSpace(jwtOptions.Issuer) || string.IsNullOrWhiteSpace(jwtOptions.Audience))
        {
            throw new InvalidOperationException("Jwt:Issuer and Jwt:Audience must be configured in appsettings.");
        }

        var keyRotationOptions = new KeyRotationOptions();
        configuration.GetSection("KeyRotation").Bind(keyRotationOptions);
        if (keyRotationOptions.OverlapMinutes < 1)
        {
            throw new InvalidOperationException("KeyRotation:OverlapMinutes must be >= 1.");
        }

        if (keyRotationOptions.MaxPublishedKeys < 1)
        {
            throw new InvalidOperationException("KeyRotation:MaxPublishedKeys must be >= 1.");
        }

        var enableJwtDiagnostics = configuration.GetValue<bool>("Jwt:EnableDiagnostics");

        var googleAuthOptions = new GoogleAuthOptions();
        configuration.GetSection("Authentication:Google").Bind(googleAuthOptions);
        if (googleAuthOptions.Enabled)
        {
            if (string.IsNullOrWhiteSpace(googleAuthOptions.ClientId))
            {
                throw new InvalidOperationException("Authentication:Google:ClientId must be configured when Google login is enabled.");
            }

            if (string.IsNullOrWhiteSpace(googleAuthOptions.ClientSecret))
            {
                throw new InvalidOperationException("Authentication:Google:ClientSecret must be configured when Google login is enabled.");
            }
        }

        var notificationOptions = new NotificationOptions();
        configuration.GetSection("Notification").Bind(notificationOptions);
        if (notificationOptions.Enabled)
        {
            if (string.IsNullOrWhiteSpace(notificationOptions.QueueConnectionString))
            {
                throw new InvalidOperationException("Notification:QueueConnectionString must be configured when notifications are enabled.");
            }

            if (string.IsNullOrWhiteSpace(notificationOptions.QueueName))
            {
                throw new InvalidOperationException("Notification:QueueName must be configured when notifications are enabled.");
            }
        }

        var dataProtectionOptions = new DataProtectionKeyManagementOptions();
        configuration.GetSection("DataProtection:KeyManagement").Bind(dataProtectionOptions);
        if (string.IsNullOrWhiteSpace(dataProtectionOptions.ApplicationName))
        {
            dataProtectionOptions.ApplicationName = "VehicleServiceBooking.Auth";
        }

        if (dataProtectionOptions.DefaultKeyLifetimeDays < 7 || dataProtectionOptions.DefaultKeyLifetimeDays > 3650)
        {
            throw new InvalidOperationException("DataProtection:KeyManagement:DefaultKeyLifetimeDays must be between 7 and 3650.");
        }

        var environmentName = configuration["ASPNETCORE_ENVIRONMENT"]
            ?? configuration["DOTNET_ENVIRONMENT"]
            ?? string.Empty;
        var isProduction = string.Equals(environmentName, "Production", StringComparison.OrdinalIgnoreCase);
        if (isProduction && string.IsNullOrWhiteSpace(dataProtectionOptions.KeyRingPath))
        {
            throw new InvalidOperationException(
                "DataProtection key ring path is required in Production. Configure DataProtection:KeyManagement:KeyRingPath to durable shared storage.");
        }

        services.AddSingleton(jwtOptions);
        services.AddSingleton(keyRotationOptions);
        services.AddSingleton(dataProtectionOptions);
        services.AddSingleton(googleAuthOptions);
        services.AddSingleton(notificationOptions);

        var dataProtectionBuilder = services.AddDataProtection()
            .SetApplicationName(dataProtectionOptions.ApplicationName)
            .SetDefaultKeyLifetime(TimeSpan.FromDays(dataProtectionOptions.DefaultKeyLifetimeDays));

        if (!string.IsNullOrWhiteSpace(dataProtectionOptions.KeyRingPath))
        {
            var keyRingDirectory = new DirectoryInfo(dataProtectionOptions.KeyRingPath);
            if (!keyRingDirectory.Exists)
            {
                keyRingDirectory.Create();
            }

            dataProtectionBuilder.PersistKeysToFileSystem(keyRingDirectory);
        }

        services.AddSingleton<ISigningKeyProvider, RsaSigningKeyProvider>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IAuthenticatorSecretProtector, DataProtectionAuthenticatorSecretProtector>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();

        if (notificationOptions.Enabled)
        {
            services.AddSingleton(_ => new QueueClient(notificationOptions.QueueConnectionString, notificationOptions.QueueName));
            services.AddSingleton<INotificationPublisher, QueueNotificationPublisher>();
        }
        else
        {
            services.AddSingleton<INotificationPublisher, NoOpNotificationPublisher>();
        }

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is required.");

        var enableDetailedErrors = configuration.GetValue<bool?>("Observability:EntityFramework:EnableDetailedErrors") ?? false;
        var enableSensitiveDataLogging = configuration.GetValue<bool?>("Observability:EntityFramework:EnableSensitiveDataLogging") ?? false;

        services.AddDbContext<AuthDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions => npgsqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(10), null));

            if (enableDetailedErrors)
            {
                options.EnableDetailedErrors();
            }

            if (enableSensitiveDataLogging)
            {
                options.EnableSensitiveDataLogging();
            }
        });

        var authenticationBuilder = services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.IncludeErrorDetails = enableJwtDiagnostics;
                if (enableJwtDiagnostics)
                {
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            var logger = context.HttpContext.RequestServices
                                .GetRequiredService<ILoggerFactory>()
                                .CreateLogger("Auth.JwtBearer");
                            logger.LogError(context.Exception, "JWT authentication failed.");
                            return Task.CompletedTask;
                        },
                        OnChallenge = context =>
                        {
                            var logger = context.HttpContext.RequestServices
                                .GetRequiredService<ILoggerFactory>()
                                .CreateLogger("Auth.JwtBearer");
                            logger.LogWarning(
                                "JWT challenge triggered. Error: {Error}. Description: {Description}",
                                context.Error,
                                context.ErrorDescription);
                            return Task.CompletedTask;
                        }
                    };
                }

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });

        if (googleAuthOptions.Enabled)
        {
            authenticationBuilder
                .AddCookie(GoogleAuthOptions.ExternalCookieScheme)
                .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
                {
                    options.SignInScheme = GoogleAuthOptions.ExternalCookieScheme;
                    options.ClientId = googleAuthOptions.ClientId;
                    options.ClientSecret = googleAuthOptions.ClientSecret;
                    options.SaveTokens = true;
                    options.CallbackPath = string.IsNullOrWhiteSpace(googleAuthOptions.CallbackPath)
                        ? "/signin-google"
                        : googleAuthOptions.CallbackPath;

                    options.ClaimActions.MapJsonKey("email_verified", "email_verified");
                    options.ClaimActions.MapJsonKey("urn:google:email_verified", "email_verified");

                    // Local development in this repository typically runs on HTTP localhost.
                    // Keep correlation cookie aligned with request scheme to avoid correlation-failed callbacks.
                    options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                    options.CorrelationCookie.SameSite = SameSiteMode.Lax;
                });
        }

        services
            .AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<ISigningKeyProvider>((jwtBearerOptions, signingKeyProvider) =>
            {
                jwtBearerOptions.TokenValidationParameters.IssuerSigningKeyResolver =
                    (token, securityToken, kid, validationParameters) => signingKeyProvider.GetValidationKeys(kid);
            });

        services.AddAuthorization();

        return services;
    }
}
