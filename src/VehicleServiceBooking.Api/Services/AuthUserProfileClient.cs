using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using VehicleServiceBooking.Api.Configuration;
using VehicleServiceBooking.Application.DTOs;
using VehicleServiceBooking.Application.Interfaces.Services;

namespace VehicleServiceBooking.Api.Services;

/// <summary>
/// HTTP client for Auth internal user core profile endpoint.
/// </summary>
public sealed class AuthUserProfileClient : IAuthUserProfileClient
{
    private readonly HttpClient _httpClient;
    private readonly AuthUserProfileOptions _options;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthUserProfileClient(HttpClient httpClient, IOptions<AuthUserProfileOptions> options, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public async Task<AuthUserCoreProfileDto?> GetUserCoreProfileByIdAsync(Guid authUserId, CancellationToken cancellationToken = default)
    {
        try
        {
            var path = _options.CoreProfilePathTemplate.Replace("{authUserId}", authUserId.ToString(), StringComparison.OrdinalIgnoreCase);
            using var request = new HttpRequestMessage(HttpMethod.Get, path);

            var rawAuthorization = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
            if (!string.IsNullOrWhiteSpace(rawAuthorization) &&
                AuthenticationHeaderValue.TryParse(rawAuthorization, out var authHeader))
            {
                request.Headers.Authorization = authHeader;
            }

            using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                throw new InvalidOperationException("Auth user is not eligible for customer bootstrap.");
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AuthUserCoreProfileDto>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException("Auth user core profile request timed out.", ex);
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException("Failed to retrieve auth user core profile from Auth service.", ex);
        }
    }
}