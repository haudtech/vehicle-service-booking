namespace VehicleServiceBooking.Auth.Services;

/// <summary>
/// Provides at-rest protection and retrieval for authenticator shared secrets.
/// </summary>
public interface IAuthenticatorSecretProtector
{
    /// <summary>
    /// Protects a plaintext authenticator secret before persistence.
    /// </summary>
    string Protect(string plaintextSecret);

    /// <summary>
    /// Attempts to recover a plaintext authenticator secret from persisted value.
    /// </summary>
    /// <param name="storedSecret">Persisted secret value.</param>
    /// <param name="plaintextSecret">Recovered plaintext secret when successful.</param>
    /// <returns>True when the secret can be used for TOTP verification; otherwise false.</returns>
    bool TryUnprotect(string storedSecret, out string plaintextSecret);
}
