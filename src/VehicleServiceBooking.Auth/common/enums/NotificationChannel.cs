namespace VehicleServiceBooking.Auth.Common.Enums;

/// <summary>
/// Supported outbound notification channels.
/// </summary>
public enum NotificationChannel
{
    Email = 0,
    Zalo = 1,
    AuthenticatorApp = 2
}

/// <summary>
/// Conversion helpers for notification channel wire values.
/// </summary>
public static class NotificationChannelExtensions
{
    /// <summary>
    /// Converts channel enum to wire representation.
    /// </summary>
    public static string ToWireValue(this NotificationChannel channel)
    {
        return channel switch
        {
            NotificationChannel.Email => "email",
            NotificationChannel.Zalo => "zalo",
            NotificationChannel.AuthenticatorApp => "authenticator_app",
            _ => "email"
        };
    }

    /// <summary>
    /// Parses wire representation to enum.
    /// </summary>
    public static NotificationChannel ParseWireValueOrDefault(string? value, NotificationChannel fallback = NotificationChannel.Email)
    {
        var normalized = value?.Trim().ToLowerInvariant();
        return normalized switch
        {
            "email" => NotificationChannel.Email,
            "zalo" => NotificationChannel.Zalo,
            "authenticator_app" => NotificationChannel.AuthenticatorApp,
            "authenticator" => NotificationChannel.AuthenticatorApp,
            _ => fallback
        };
    }
}