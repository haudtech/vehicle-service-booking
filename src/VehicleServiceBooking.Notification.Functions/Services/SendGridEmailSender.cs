using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VehicleServiceBooking.Notification.Functions.Configuration;
using VehicleServiceBooking.Notification.Functions.Models;

namespace VehicleServiceBooking.Notification.Functions.Services;

/// <summary>
/// Production email sender using SendGrid Mail Send API.
/// </summary>
public sealed class SendGridEmailSender : IEmailSender
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _httpClient;
    private readonly EmailSenderOptions _options;
    private readonly ILogger<SendGridEmailSender> _logger;

    public SendGridEmailSender(
        IHttpClientFactory httpClientFactory,
        IOptions<EmailSenderOptions> options,
        ILogger<SendGridEmailSender> logger)
    {
        _httpClient = httpClientFactory.CreateClient(nameof(SendGridEmailSender));
        _options = options.Value;
        _logger = logger;
    }

    public async Task SendAsync(NotificationMessage message, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.SendGridApiKey))
        {
            throw new InvalidOperationException("EmailSender:SendGridApiKey must be configured when provider is SendGrid.");
        }

        if (string.IsNullOrWhiteSpace(_options.FromAddress))
        {
            throw new InvalidOperationException("EmailSender:FromAddress must be configured when provider is SendGrid.");
        }

        var payload = new
        {
            from = new
            {
                email = _options.FromAddress,
                name = string.IsNullOrWhiteSpace(_options.FromName) ? "Vehicle Service Booking" : _options.FromName
            },
            personalizations = new[]
            {
                new
                {
                    to = new[]
                    {
                        new { email = message.ToEmail }
                    },
                    subject = message.Subject
                }
            },
            content = BuildContentParts(message)
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, _options.SendGridEndpoint)
        {
            Content = new StringContent(JsonSerializer.Serialize(payload, JsonOptions), Encoding.UTF8, "application/json")
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.SendGridApiKey);

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        response.Headers.TryGetValues("X-Message-Id", out var messageIdValues);
        var responseMessageId = messageIdValues?.FirstOrDefault() ?? "<missing>";

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError(
                "SendGrid email send failed. StatusCode={StatusCode}, ReasonPhrase={ReasonPhrase}, SendGridMessageId={SendGridMessageId}, ToEmail={ToEmail}, CorrelationId={CorrelationId}, Body={Body}",
                (int)response.StatusCode,
                response.ReasonPhrase,
                responseMessageId,
                message.ToEmail,
                message.CorrelationId,
                responseBody);

            throw new InvalidOperationException($"SendGrid email send failed with status {(int)response.StatusCode}.");
        }

        _logger.LogInformation(
            "SendGrid email sent. StatusCode={StatusCode}, ReasonPhrase={ReasonPhrase}, SendGridMessageId={SendGridMessageId}, ToEmail={ToEmail}, EventType={EventType}, CorrelationId={CorrelationId}, Body={Body}",
            (int)response.StatusCode,
            response.ReasonPhrase,
            responseMessageId,
            message.ToEmail,
            message.EventType,
            message.CorrelationId,
            responseBody);
    }

    private static object[] BuildContentParts(NotificationMessage message)
    {
        if (string.IsNullOrWhiteSpace(message.HtmlContent))
        {
            return
            [
                new
                {
                    type = "text/plain",
                    value = message.Content
                }
            ];
        }

        return
        [
            new
            {
                type = "text/plain",
                value = message.Content
            },
            new
            {
                type = "text/html",
                value = message.HtmlContent
            }
        ];
    }
}
