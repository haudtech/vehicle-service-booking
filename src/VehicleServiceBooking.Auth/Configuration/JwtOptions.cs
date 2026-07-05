namespace VehicleServiceBooking.Auth.Configuration;

/// <summary>
/// Configurable options for JWT issuance and validation.
/// </summary>
public sealed class JwtOptions
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int AccessTokenLifetimeMinutes { get; set; } = 30;
    public int RefreshTokenLifetimeDays { get; set; } = 30;
}
