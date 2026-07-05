namespace VehicleServiceBooking.Auth.Models.Responses;

/// <summary>
/// Response payload containing issued access and refresh tokens.
/// </summary>
public sealed class AuthResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string TokenType { get; set; } = "Bearer";
}
