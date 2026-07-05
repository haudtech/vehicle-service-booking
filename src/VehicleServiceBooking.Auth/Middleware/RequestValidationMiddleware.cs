using System.Text.Json;

namespace VehicleServiceBooking.Auth.Middleware;

/// <summary>
/// Validates request content type and JSON payload shape before reaching controllers.
/// </summary>
public sealed class RequestValidationMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestValidationMiddleware"/> class.
    /// </summary>
    /// <param name="next">Next middleware in the pipeline.</param>
    public RequestValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Invokes middleware logic for content-type and JSON validation.
    /// </summary>
    /// <param name="context">Current HTTP context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        if (HttpMethods.IsPost(context.Request.Method) || HttpMethods.IsPut(context.Request.Method) || HttpMethods.IsPatch(context.Request.Method))
        {
            if (string.IsNullOrWhiteSpace(context.Request.ContentType) || !context.Request.ContentType.Contains("application/json", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = StatusCodes.Status415UnsupportedMediaType;
                await context.Response.WriteAsJsonAsync(new { message = "Invalid Content-Type. Use application/json." });
                return;
            }
        }

        try
        {
            await _next(context);
        }
        catch (JsonException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { message = "Invalid JSON format." });
        }
    }
}
