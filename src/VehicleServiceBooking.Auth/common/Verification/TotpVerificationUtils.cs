using System.Security.Cryptography;
using System.Text;

namespace VehicleServiceBooking.Auth.Common.Verification;

/// <summary>
/// Utility for verifying RFC6238 TOTP codes using a shared Base32 secret.
/// </summary>
public static class TotpVerificationUtils
{
    private const int TimeStepSeconds = 30;
    private const int TotpDigits = 6;

    /// <summary>
    /// Verifies a TOTP code against the supplied Base32 secret.
    /// Accepts +/- one time-step drift.
    /// </summary>
    public static bool VerifyCode(string base32Secret, string code)
    {
        if (string.IsNullOrWhiteSpace(base32Secret) || string.IsNullOrWhiteSpace(code))
        {
            return false;
        }

        var normalizedCode = code.Trim();
        if (normalizedCode.Length != TotpDigits || !normalizedCode.All(char.IsDigit))
        {
            return false;
        }

        var key = DecodeBase32(base32Secret);
        if (key.Length == 0)
        {
            return false;
        }

        var unixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var counter = unixTime / TimeStepSeconds;

        for (long drift = -1; drift <= 1; drift++)
        {
            var expected = ComputeTotp(key, counter + drift);
            if (CryptographicOperations.FixedTimeEquals(
                    Encoding.UTF8.GetBytes(expected),
                    Encoding.UTF8.GetBytes(normalizedCode)))
            {
                return true;
            }
        }

        return false;
    }

    private static string ComputeTotp(byte[] key, long counter)
    {
        Span<byte> counterBytes = stackalloc byte[8];
        for (var i = 7; i >= 0; i--)
        {
            counterBytes[i] = (byte)(counter & 0xff);
            counter >>= 8;
        }

        using var hmac = new HMACSHA1(key);
        var hash = hmac.ComputeHash(counterBytes.ToArray());
        var offset = hash[^1] & 0x0f;

        var binaryCode = ((hash[offset] & 0x7f) << 24)
                         | (hash[offset + 1] << 16)
                         | (hash[offset + 2] << 8)
                         | hash[offset + 3];

        var otp = binaryCode % (int)Math.Pow(10, TotpDigits);
        return otp.ToString($"D{TotpDigits}");
    }

    private static byte[] DecodeBase32(string input)
    {
        var normalized = input.Trim().Replace("=", string.Empty).Replace(" ", string.Empty).ToUpperInvariant();
        if (normalized.Length == 0)
        {
            return [];
        }

        const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        var output = new List<byte>();

        var bitBuffer = 0;
        var bitsInBuffer = 0;

        foreach (var c in normalized)
        {
            var value = alphabet.IndexOf(c);
            if (value < 0)
            {
                return [];
            }

            bitBuffer = (bitBuffer << 5) | value;
            bitsInBuffer += 5;

            if (bitsInBuffer >= 8)
            {
                bitsInBuffer -= 8;
                output.Add((byte)((bitBuffer >> bitsInBuffer) & 0xff));
            }
        }

        return output.ToArray();
    }
}
