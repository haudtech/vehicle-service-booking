using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace VehicleServiceBooking.Api.Common.Utils;

public static class EncodeUtils
{
    /// <summary>
    /// Computes a deterministic SHA-256 hash from the JSON-serialized request payload.
    /// </summary>
    /// <typeparam name="TRequest">The request payload type to hash.</typeparam>
    /// <param name="request">The request object used as the hash source.</param>
    /// <returns>An uppercase hexadecimal SHA-256 hash string.</returns>
    public static string ComputeRequestHash<TRequest>(TRequest request)
    {
        var canonicalJson = JsonSerializer.Serialize(request);
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(canonicalJson));
        return Convert.ToHexString(hashBytes);
    }
}