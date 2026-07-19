using System;

namespace VehicleServiceBooking.Api.Common.Utils;

public static class OrderCodeGenerator
{
    /// <summary>
    /// Generates a unique order code using the ORD prefix, UTC timestamp, and GUID suffix.
    /// </summary>
    /// <returns>An order code in the format ORD-yyyyMMddHHmmssfff-xxxxxxxxxxxx.</returns>
    public static string GenerateOrderCode()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
        var suffix = Guid.NewGuid().ToString("N")[..12];
        return $"ORD-{timestamp}-{suffix}";
    }
}
