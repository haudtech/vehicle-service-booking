using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VehicleServiceBooking.Notification.Functions.Configuration;
using VehicleServiceBooking.Notification.Functions.Models;

namespace VehicleServiceBooking.Notification.Functions.Services;

/// <summary>
/// Production email sender using Gmail API.
/// </summary>
public sealed class GoogleEmailSender : IEmailSender
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _httpClient;
    private readonly EmailSenderOptions _options;
    private readonly ILogger<GoogleEmailSender> _logger;

    public GoogleEmailSender(
        IHttpClientFactory httpClientFactory,
        IOptions<EmailSenderOptions> options,
        ILogger<GoogleEmailSender> logger)
    {
        _httpClient = httpClientFactory.CreateClient(nameof(GoogleEmailSender));
        _options = options.Value;
        _logger = logger;
    }

    public async Task SendAsync(NotificationMessage message, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.FromAddress))
        {
            throw new InvalidOperationException("EmailSender:FromAddress must be configured when provider is Google.");
        }

        var accessToken = await ResolveAccessTokenAsync(cancellationToken);
        var userId = string.IsNullOrWhiteSpace(_options.GoogleUserId) ? "me" : _options.GoogleUserId;
        var endpoint = $"{_options.GoogleApiEndpoint.TrimEnd('/')}/{Uri.EscapeDataString(userId)}/messages/send";

        var mimeMessage = BuildMimeMessage(message);
        var encodedMime = Convert.ToBase64String(Encoding.UTF8.GetBytes(mimeMessage))
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');

        var payload = new { raw = encodedMime };

        using var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = new StringContent(JsonSerializer.Serialize(payload, JsonOptions), Encoding.UTF8, "application/json")
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        string? providerMessageId = null;
        string? threadId = null;

        try
        {
            using var doc = JsonDocument.Parse(responseBody);
            if (doc.RootElement.TryGetProperty("id", out var idElement))
            {
                providerMessageId = idElement.GetString();
            }

            if (doc.RootElement.TryGetProperty("threadId", out var threadIdElement))
            {
                threadId = threadIdElement.GetString();
            }
        }
        catch (JsonException)
        {
            // Keep raw body logging; no additional handling needed.
        }

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError(
                "Google Gmail API email send failed. StatusCode={StatusCode}, ReasonPhrase={ReasonPhrase}, ProviderMessageId={ProviderMessageId}, ThreadId={ThreadId}, ToEmail={ToEmail}, CorrelationId={CorrelationId}, Body={Body}",
                (int)response.StatusCode,
                response.ReasonPhrase,
                providerMessageId ?? "<missing>",
                threadId ?? "<missing>",
                message.ToEmail,
                message.CorrelationId,
                responseBody);

            throw new InvalidOperationException($"Google Gmail API email send failed with status {(int)response.StatusCode}.");
        }

        _logger.LogInformation(
            "Google Gmail API email sent. StatusCode={StatusCode}, ReasonPhrase={ReasonPhrase}, ProviderMessageId={ProviderMessageId}, ThreadId={ThreadId}, ToEmail={ToEmail}, EventType={EventType}, CorrelationId={CorrelationId}, Body={Body}",
            (int)response.StatusCode,
            response.ReasonPhrase,
            providerMessageId ?? "<missing>",
            threadId ?? "<missing>",
            message.ToEmail,
            message.EventType,
            message.CorrelationId,
            responseBody);
    }

    private async Task<string> ResolveAccessTokenAsync(CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(_options.GoogleAccessToken))
        {
            return _options.GoogleAccessToken;
        }

        if (string.IsNullOrWhiteSpace(_options.GoogleClientId)
            || string.IsNullOrWhiteSpace(_options.GoogleClientSecret)
            || string.IsNullOrWhiteSpace(_options.GoogleRefreshToken))
        {
            throw new InvalidOperationException(
                "Google provider requires EmailSender:GoogleAccessToken or the full refresh-token trio: EmailSender:GoogleClientId, EmailSender:GoogleClientSecret, EmailSender:GoogleRefreshToken.");
        }

        using var tokenRequest = new HttpRequestMessage(HttpMethod.Post, _options.GoogleTokenEndpoint)
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = _options.GoogleClientId,
                ["client_secret"] = _options.GoogleClientSecret,
                ["refresh_token"] = _options.GoogleRefreshToken,
                ["grant_type"] = "refresh_token"
            })
        };

        using var tokenResponse = await _httpClient.SendAsync(tokenRequest, cancellationToken);
        var tokenBody = await tokenResponse.Content.ReadAsStringAsync(cancellationToken);

        if (!tokenResponse.IsSuccessStatusCode)
        {
            _logger.LogError(
                "Google OAuth token request failed. StatusCode={StatusCode}, ReasonPhrase={ReasonPhrase}, Body={Body}",
                (int)tokenResponse.StatusCode,
                tokenResponse.ReasonPhrase,
                tokenBody);
            throw new InvalidOperationException($"Google OAuth token request failed with status {(int)tokenResponse.StatusCode}.");
        }

        try
        {
            using var doc = JsonDocument.Parse(tokenBody);
            if (doc.RootElement.TryGetProperty("access_token", out var accessTokenElement)
                && !string.IsNullOrWhiteSpace(accessTokenElement.GetString()))
            {
                return accessTokenElement.GetString()!;
            }
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("Google OAuth token response is not valid JSON.", ex);
        }

        throw new InvalidOperationException("Google OAuth token response did not include access_token.");
    }

    private string BuildMimeMessage(NotificationMessage message)
    {
        var fromName = string.IsNullOrWhiteSpace(_options.FromName)
            ? "Vehicle Service Booking"
            : _options.FromName;

        var builder = new StringBuilder();
        builder.Append("From: \"").Append(fromName.Replace("\"", "'")).Append("\" <").Append(_options.FromAddress).AppendLine(">\r");
        builder.Append("To: <").Append(message.ToEmail).AppendLine(">\r");
        builder.Append("Subject: ").AppendLine(message.Subject);
        builder.AppendLine("MIME-Version: 1.0");
        builder.AppendLine("Content-Type: text/plain; charset=utf-8");
        builder.AppendLine("Content-Transfer-Encoding: 8bit");
        builder.AppendLine();
        builder.Append(message.Content ?? string.Empty);
        return builder.ToString();
    }
}