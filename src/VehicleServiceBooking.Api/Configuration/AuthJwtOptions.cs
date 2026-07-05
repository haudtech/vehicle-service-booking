namespace VehicleServiceBooking.Api.Configuration;

public sealed class AuthJwtOptions
{
    public const string SectionName = "AuthJwt";

    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string JwksUrl { get; set; } = string.Empty;
    public int JwksCacheMinutes { get; set; } = 15;
    public bool EnableDiagnostics { get; set; }
}