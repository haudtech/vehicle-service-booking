namespace VehicleServiceBooking.Application.Configuration.Interfaces;

/// <summary>
/// Configuration interface for CORS (Cross-Origin Resource Sharing) settings
/// Defines the contract for CORS policies that can be applied to the API
/// </summary>
public interface ICorsConfiguration
{
    /// <summary>
    /// Gets the allowed origins (domains that can make requests to the API)
    /// Example: ["https://example.com", "https://app.example.com"]
    /// In development, typically includes localhost variants
    /// </summary>
    string[] AllowedOrigins { get; }

    /// <summary>
    /// Gets the allowed HTTP methods
    /// Example: ["GET", "POST", "PUT", "DELETE", "OPTIONS"]
    /// </summary>
    string[] AllowedMethods { get; }

    /// <summary>
    /// Gets the allowed HTTP headers that clients can send
    /// Example: ["Content-Type", "Authorization", "Accept"]
    /// </summary>
    string[] AllowedHeaders { get; }

    /// <summary>
    /// Gets the exposed response headers that browsers are allowed to access
    /// Example: ["X-Total-Count", "X-Page-Number"]
    /// </summary>
    string[] ExposedHeaders { get; }

    /// <summary>
    /// Gets the maximum age in seconds for which preflight response can be cached
    /// Reduces the number of preflight requests
    /// Typical value: 3600 (1 hour) for production
    /// </summary>
    int MaxAge { get; }

    /// <summary>
    /// Gets whether credentials (cookies, HTTP authentication) are allowed
    /// If true, AllowedOrigins cannot contain "*" (must be specific origins)
    /// </summary>
    bool AllowCredentials { get; }

    /// <summary>
    /// Gets the CORS policy name used for applying with [EnableCors] attribute
    /// Example: "DevelopmentPolicy", "ProductionPolicy"
    /// </summary>
    string PolicyName { get; }
}
