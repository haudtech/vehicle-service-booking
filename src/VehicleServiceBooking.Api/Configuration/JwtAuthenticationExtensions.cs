using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace VehicleServiceBooking.Api.Configuration;

public static class JwtAuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var authJwtOptions = configuration
            .GetSection(AuthJwtOptions.SectionName)
            .Get<AuthJwtOptions>() ?? new AuthJwtOptions();

        if (string.IsNullOrWhiteSpace(authJwtOptions.Issuer) ||
            string.IsNullOrWhiteSpace(authJwtOptions.Audience) ||
            string.IsNullOrWhiteSpace(authJwtOptions.JwksUrl))
        {
            throw new InvalidOperationException(
                "AuthJwt:Issuer, AuthJwt:Audience, and AuthJwt:JwksUrl must be configured.");
        }

        services.AddSingleton(authJwtOptions);
        services.AddHttpClient("auth-jwks");
        services.AddSingleton<IJwksSigningKeyProvider, JwksSigningKeyProvider>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.IncludeErrorDetails = authJwtOptions.EnableDiagnostics;
                if (authJwtOptions.EnableDiagnostics)
                {
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            var logger = context.HttpContext.RequestServices
                                .GetRequiredService<ILoggerFactory>()
                                .CreateLogger("Booking.JwtBearer");
                            logger.LogError(context.Exception, "Booking API JWT authentication failed.");
                            return Task.CompletedTask;
                        }
                    };
                }

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = authJwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = authJwtOptions.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.FromMinutes(1),
                    NameClaimType = "sub",
                    RoleClaimType = "roles"
                };
            });

        services
            .AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IJwksSigningKeyProvider>((options, jwksProvider) =>
            {
                options.TokenValidationParameters.IssuerSigningKeyResolver =
                    (token, securityToken, kid, validationParameters) => jwksProvider.GetSigningKeys(kid);
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("AppointmentCreatePolicy", policy =>
                policy.RequireClaim("permissions", "appointment:create"));
            options.AddPolicy("AppointmentCompletePolicy", policy =>
                policy.RequireClaim("permissions", "appointment:complete"));
            options.AddPolicy("AppointmentReadPolicy", policy =>
                policy.RequireClaim("permissions", "appointment:view"));
        });

        return services;
    }
}