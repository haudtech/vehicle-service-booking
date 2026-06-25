using System;

namespace VehicleServiceBooking.Application.Configuration;

/// <summary>
/// CORS configuration options for different environments
/// Implements ICorsConfiguration and maps from appsettings.json
/// </summary>
public class CorsOptions : ICorsConfiguration
{
    /// <summary>
    /// Configuration section name for binding from appsettings
    /// Usage: builder.Services.Configure<CorsOptions>(
    ///   builder.Configuration.GetSection(CorsOptions.SectionName))
    /// </summary>
    public const string SectionName = "Cors";

    /// <summary>
    /// Gets or sets the allowed origins (domains that can make requests to the API)
    /// 
    /// Development: ["http://localhost:3000", "http://localhost:5173", "http://127.0.0.1:*"]
    /// Staging:     ["https://staging.example.com"]
    /// Production:  ["https://app.example.com", "https://example.com"]
    /// 
    /// IMPORTANT: Never use "*" with AllowCredentials=true
    /// </summary>
    public string[] AllowedOrigins { get; set; } = new[] { "http://localhost:*", "http://127.0.0.1:*" };

    /// <summary>
    /// Gets or sets the allowed HTTP methods
    /// Standard: GET, POST, PUT, DELETE, PATCH, OPTIONS
    /// Always include OPTIONS for preflight requests
    /// </summary>
    public string[] AllowedMethods { get; set; } = new[] { "GET", "POST", "PUT", "DELETE", "PATCH", "OPTIONS" };

    /// <summary>
    /// Gets or sets the allowed request headers from clients
    /// 
    /// Common headers:
    /// - "Content-Type" - For specifying request body format (application/json, etc)
    /// - "Authorization" - For passing authentication tokens (Bearer tokens, etc)
    /// - "Accept" - For specifying expected response format
    /// - "Accept-Language" - For language preference
    /// 
    /// "*" allows any header (simple case)
    /// For specific headers, list them explicitly
    /// </summary>
    public string[] AllowedHeaders { get; set; } = new[] { "*" };

    /// <summary>
    /// Gets or sets the response headers that browsers are allowed to access
    /// By default, browsers can only access simple headers (Content-Type, etc)
    /// Use this to expose custom headers
    /// 
    /// Example exposed headers for pagination:
    /// - "X-Total-Count" - Total number of resources
    /// - "X-Page-Number" - Current page number
    /// - "X-Page-Size" - Items per page
    /// 
    /// Example exposed headers for rate limiting:
    /// - "X-RateLimit-Limit" - Total requests allowed
    /// - "X-RateLimit-Remaining" - Requests remaining
    /// - "X-RateLimit-Reset" - When limit resets
    /// </summary>
    public string[] ExposedHeaders { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Gets or sets the maximum age in seconds for which preflight responses are cached
    /// 
    /// Preflight request: Browser sends OPTIONS before actual request
    /// Cache allows browser to skip preflight for subsequent requests
    /// 
    /// Recommended values:
    /// - Development:   300 seconds (5 minutes) - Short for frequent changes
    /// - Staging:       1800 seconds (30 minutes) - Medium for stability testing
    /// - Production:    3600 seconds (1 hour) - Long to reduce requests
    /// 
    /// Set to 0 to disable caching (preflight on every request)
    /// </summary>
    public int MaxAge { get; set; } = 3600;

    /// <summary>
    /// Gets or sets whether credentials (cookies, auth headers) are allowed in CORS requests
    /// 
    /// If true:
    /// - Cookies and HTTP authentication will be included with requests
    /// - AllowedOrigins cannot be "*" (must specify exact origins)
    /// - Client must set credentials mode: include in fetch/AJAX
    /// 
    /// If false:
    /// - No credentials sent automatically
    /// - AllowedOrigins can use "*"
    /// - Suitable for public APIs
    /// 
    /// Typical values:
    /// - Development:   true (for testing authentication flows)
    /// - Production:    true (if using session/cookie auth), false (if using only token auth)
    /// </summary>
    public bool AllowCredentials { get; set; } = false;

    /// <summary>
    /// Gets or sets the policy name for applying CORS with [EnableCors] attribute
    /// Example: [EnableCors("DevelopmentPolicy")]
    /// 
    /// Default names:
    /// - Development: "DevelopmentCorsPolicy"
    /// - Staging:     "StagingCorsPolicy"  
    /// - Production:  "ProductionCorsPolicy"
    /// </summary>
    public string PolicyName { get; set; } = "DefaultCorsPolicy";
}
