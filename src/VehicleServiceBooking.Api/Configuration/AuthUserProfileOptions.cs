namespace VehicleServiceBooking.Api.Configuration;

using System;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Configuration for API-to-Auth core profile lookups.
/// </summary>
public sealed class AuthUserProfileOptions
{
    public const string SectionName = "AuthUserProfile";

    /// <summary>
    /// Backward-compatible absolute base URL, for example: https://auth.company.com.
    /// If this is set, it takes precedence over Scheme/Host/Port/BasePath.
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Endpoint scheme when using host-based configuration. Defaults to https when omitted.
    /// </summary>
    public string? Scheme { get; set; }

    /// <summary>
    /// Endpoint hostname when using host-based configuration.
    /// </summary>
    public string? Host { get; set; }

    /// <summary>
    /// Optional endpoint port when using host-based configuration.
    /// </summary>
    [Range(1, 65535)]
    public int? Port { get; set; }

    /// <summary>
    /// Optional base path prefix for auth endpoints, for example: /auth.
    /// </summary>
    public string? BasePath { get; set; }

    [Required]
    public required string CoreProfilePathTemplate { get; set; }

    [Range(1, 300)]
    public int TimeoutSeconds { get; set; }

    public Uri GetBaseAddress()
    {
        if (!string.IsNullOrWhiteSpace(BaseUrl))
        {
            return new Uri(BaseUrl, UriKind.Absolute);
        }

        if (string.IsNullOrWhiteSpace(Host))
        {
            throw new InvalidOperationException(
                "AuthUserProfile endpoint is invalid. Configure either BaseUrl or Host (with optional Scheme/Port/BasePath).");
        }

        var uriBuilder = new UriBuilder
        {
            Scheme = string.IsNullOrWhiteSpace(Scheme) ? Uri.UriSchemeHttps : Scheme,
            Host = Host.Trim()
        };

        if (Port.HasValue)
        {
            uriBuilder.Port = Port.Value;
        }

        if (!string.IsNullOrWhiteSpace(BasePath))
        {
            uriBuilder.Path = BasePath!;
        }

        return uriBuilder.Uri;
    }
}