namespace VehicleServiceBooking.Auth.Common.Enums;

/// <summary>
/// Supported login challenge delivery and verification channels.
/// </summary>
public enum ChallengeChannel
{
    OtpFirst = 0,
    EmailOtp = 1,
    AuthenticatorApp = 2
}

/// <summary>
/// Conversion helpers for challenge channel wire/storage values.
/// </summary>
public static class ChallengeChannelExtensions
{
    /// <summary>
    /// Converts channel enum to the API/storage string representation.
    /// </summary>
    public static string ToWireValue(this ChallengeChannel channel)
    {
        return channel switch
        {
            ChallengeChannel.OtpFirst => "otp_first",
            ChallengeChannel.EmailOtp => "email_otp",
            ChallengeChannel.AuthenticatorApp => "authenticator_app",
            _ => "email_otp"
        };
    }

    /// <summary>
    /// Parses API/storage string into enum; returns false when invalid.
    /// </summary>
    public static bool TryParseWireValue(string? value, out ChallengeChannel channel)
    {
        var normalized = value?.Trim().ToLowerInvariant();
        channel = normalized switch
        {
            "otp_first" => ChallengeChannel.OtpFirst,
            "email_otp" => ChallengeChannel.EmailOtp,
            "authenticator_app" => ChallengeChannel.AuthenticatorApp,
            _ => ChallengeChannel.EmailOtp
        };

        return normalized is "otp_first" or "email_otp" or "authenticator_app";
    }

    /// <summary>
    /// Parses API/storage string into enum; returns provided fallback when invalid.
    /// </summary>
    public static ChallengeChannel ParseWireValueOrDefault(string? value, ChallengeChannel fallback = ChallengeChannel.EmailOtp)
    {
        return TryParseWireValue(value, out var parsed) ? parsed : fallback;
    }
}
