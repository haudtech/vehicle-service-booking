using System.Security.Cryptography;
using System.Text;

namespace VehicleServiceBooking.Auth.Common.Verification;

/// <summary>
/// Utilities for authenticator-app enrollment material generation.
/// </summary>
public static class AuthenticatorSetupUtils
{
    /// <summary>
    /// Generates a random Base32-encoded secret for TOTP enrollment.
    /// </summary>
    public static string GenerateBase32Secret(int byteLength = 20)
    {
        var bytes = RandomNumberGenerator.GetBytes(byteLength);
        return EncodeBase32(bytes);
    }

    /// <summary>
    /// Builds otpauth URI for TOTP clients.
    /// </summary>
    public static string BuildOtpAuthUri(string issuer, string accountName, string base32Secret, string algorithm = "SHA1", int digits = 6, int periodSeconds = 30)
    {
        var normalizedIssuer = string.IsNullOrWhiteSpace(issuer) ? "VehicleServiceBooking" : issuer.Trim();
        var normalizedAccount = string.IsNullOrWhiteSpace(accountName) ? "user" : accountName.Trim();
        var label = Uri.EscapeDataString($"{normalizedIssuer}:{normalizedAccount}");
        var issuerParam = Uri.EscapeDataString(normalizedIssuer);

        return $"otpauth://totp/{label}?secret={base32Secret}&issuer={issuerParam}&algorithm={algorithm}&digits={digits}&period={periodSeconds}";
    }

    /// <summary>
    /// Returns masked secret string to avoid exposing full secret in responses.
    /// </summary>
    public static string MaskSecret(string secret)
    {
        if (string.IsNullOrWhiteSpace(secret) || secret.Length <= 8)
        {
            return "********";
        }

        return $"{secret[..4]}...{secret[^4..]}";
    }

    private static string EncodeBase32(byte[] data)
    {
        const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        var output = new StringBuilder((data.Length + 4) / 5 * 8);

        var bitBuffer = 0;
        var bitsInBuffer = 0;

        foreach (var b in data)
        {
            bitBuffer = (bitBuffer << 8) | b;
            bitsInBuffer += 8;

            while (bitsInBuffer >= 5)
            {
                bitsInBuffer -= 5;
                var index = (bitBuffer >> bitsInBuffer) & 0x1F;
                output.Append(alphabet[index]);
            }
        }

        if (bitsInBuffer > 0)
        {
            var index = (bitBuffer << (5 - bitsInBuffer)) & 0x1F;
            output.Append(alphabet[index]);
        }

        return output.ToString();
    }
}
