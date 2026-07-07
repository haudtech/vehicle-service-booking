using Microsoft.AspNetCore.Authentication.JwtBearer;
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

        services.AddSingleton(jwtOptions);
        services.AddSingleton(keyRotationOptions);
        services.AddSingleton<ISigningKeyProvider, RsaSigningKeyProvider>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is required.");

        services.AddDbContext<AuthDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions => npgsqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(10), null));
        });

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
