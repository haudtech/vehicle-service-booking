using Microsoft.AspNetCore.DataProtection;

namespace VehicleServiceBooking.Auth.Services;

/// <summary>
/// Uses ASP.NET Core Data Protection to encrypt/decrypt authenticator shared secrets.
/// </summary>
public sealed class DataProtectionAuthenticatorSecretProtector : IAuthenticatorSecretProtector
{
    private const string ProtectedPrefix = "enc:v1:";
    private readonly IDataProtector _protector;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataProtectionAuthenticatorSecretProtector"/> class.
    /// </summary>
    public DataProtectionAuthenticatorSecretProtector(IDataProtectionProvider dataProtectionProvider)
    {
        _protector = dataProtectionProvider.CreateProtector("VehicleServiceBooking.Auth.AuthenticatorSecret.v1");
    }

    /// <inheritdoc />
    public string Protect(string plaintextSecret)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(plaintextSecret);
        return ProtectedPrefix + _protector.Protect(plaintextSecret.Trim());
    }

    /// <inheritdoc />
    public bool TryUnprotect(string storedSecret, out string plaintextSecret)
    {
        plaintextSecret = string.Empty;
        if (string.IsNullOrWhiteSpace(storedSecret))
        {
            return false;
        }

        // Backward compatibility for previously stored plaintext secrets.
        if (!storedSecret.StartsWith(ProtectedPrefix, StringComparison.Ordinal))
        {
            plaintextSecret = storedSecret;
            return true;
        }

        try
        {
            var payload = storedSecret[ProtectedPrefix.Length..];
            plaintextSecret = _protector.Unprotect(payload);
            return !string.IsNullOrWhiteSpace(plaintextSecret);
        }
        catch
        {
            return false;
        }
    }
}
