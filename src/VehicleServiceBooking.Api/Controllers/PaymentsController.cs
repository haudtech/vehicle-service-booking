using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VehicleServiceBooking.Application.Configuration;
using VehicleServiceBooking.Application.DTOs;
using VehicleServiceBooking.Application.Exceptions;
using VehicleServiceBooking.Application.Helpers;
using VehicleServiceBooking.Application.Interfaces.Services;
using VehicleServiceBooking.Domain.Enums;

namespace VehicleServiceBooking.Api.Controllers;

/// <summary>
/// Manages payment integration endpoints.
/// </summary>
[ApiController]
[Route("api/v1/payments")]
[Produces("application/json")]
[Tags("Payments")]
public sealed class PaymentsController : ControllerBase
{
    private readonly IValidator<ProcessPaymentWebhookRequest> _processWebhookValidator;
    private readonly IPaymentWebhookService _paymentWebhookService;
    private readonly IZaloPayWebhookIngressService _zaloPayWebhookIngressService;
    private readonly PaymentWebhookSecurityOptions _webhookSecurityOptions;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(
        IValidator<ProcessPaymentWebhookRequest> processWebhookValidator,
        IPaymentWebhookService paymentWebhookService,
        IZaloPayWebhookIngressService zaloPayWebhookIngressService,
        IOptions<PaymentWebhookSecurityOptions> webhookSecurityOptions,
        ILogger<PaymentsController> logger)
    {
        _processWebhookValidator = processWebhookValidator ?? throw new ArgumentNullException(nameof(processWebhookValidator));
        _paymentWebhookService = paymentWebhookService ?? throw new ArgumentNullException(nameof(paymentWebhookService));
        _zaloPayWebhookIngressService = zaloPayWebhookIngressService ?? throw new ArgumentNullException(nameof(zaloPayWebhookIngressService));
        _webhookSecurityOptions = webhookSecurityOptions?.Value ?? throw new ArgumentNullException(nameof(webhookSecurityOptions));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Optional browser return endpoint after provider checkout redirect.
    /// Webhook processing remains the source of truth for payment finalization.
    /// </summary>
    [HttpGet("return")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult PaymentReturn(
        [FromQuery] string? intentCode,
        [FromQuery] string? transactionStatus,
        [FromQuery] string? providerTransactionId,
        [FromQuery] string? eventId)
    {
        return Ok(new
        {
            message = "Payment return acknowledged. Final payment state is confirmed via webhook processing.",
            intentCode = intentCode?.Trim(),
            transactionStatus = transactionStatus?.Trim(),
            providerTransactionId = providerTransactionId?.Trim(),
            eventId = eventId?.Trim(),
            returnedAtUtc = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Development-only mock checkout page to simulate provider redirect flow.
    /// </summary>
    [HttpGet("mock/checkout/{providerType}/{intentCode}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult MockCheckout(
        [FromRoute] PaymentProviderType providerType,
        [FromRoute] string intentCode)
    {
        var safeIntentCode = string.IsNullOrWhiteSpace(intentCode) ? "<missing-intent-code>" : intentCode.Trim();
        var html = $"""
<!doctype html>
<html>
<head><meta charset='utf-8'><title>Mock Checkout</title></head>
<body style='font-family: sans-serif; margin: 24px;'>
  <h2>Mock Checkout ({providerType})</h2>
  <p>IntentCode: <strong>{safeIntentCode}</strong></p>
  <p>This page simulates a provider checkout confirmation in development.</p>
  <form method='post' action='/api/v1/payments/mock/checkout/{providerType}/{safeIntentCode}/complete'>
    <label>TransactionStatus:
      <select name='transactionStatus'>
        <option value='Completed' selected>Completed</option>
        <option value='Failed'>Failed</option>
        <option value='Expired'>Expired</option>
        <option value='Cancelled'>Cancelled</option>
      </select>
    </label>
    <button type='submit' style='margin-left:8px;'>Complete Mock Payment</button>
  </form>
</body>
</html>
""";

        return Content(html, "text/html");
    }

    /// <summary>
    /// Development-only callback trigger that simulates provider webhook callback.
    /// </summary>
    [HttpPost("mock/checkout/{providerType}/{intentCode}/complete")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ProcessPaymentWebhookResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProcessPaymentWebhookResponse>> CompleteMockCheckout(
        [FromRoute] PaymentProviderType providerType,
        [FromRoute] string intentCode,
        [FromForm] string? transactionStatus,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(intentCode))
        {
            return BadRequest(new ErrorResponse
            {
                Message = "Intent code cannot be empty",
                ErrorCode = "INVALID_INTENT_CODE",
                Timestamp = DateTime.UtcNow
            });
        }

        if (string.IsNullOrWhiteSpace(transactionStatus))
        {
            return BadRequest(new ErrorResponse
            {
                Message = "Transaction status cannot be empty",
                ErrorCode = "INVALID_TRANSACTION_STATUS",
                Timestamp = DateTime.UtcNow
            });
        }

        var request = new ProcessPaymentWebhookRequest
        {
            EventId = $"mock-{Guid.NewGuid():N}",
            IntentCode = intentCode.Trim(),
            TransactionStatus = transactionStatus.Trim(),
            ProviderTransactionId = $"mock-tx-{Guid.NewGuid():N}"[..20],
            OccurredAtUtc = DateTime.UtcNow,
            Payload = "{\"source\":\"mock-checkout\"}"
        };

        request.SignatureHash = ComputeMockSignature(providerType, request);

        await _processWebhookValidator
            .ValidateAndThrowAsync(request, cancellationToken)
            .ConfigureAwait(false);

        var result = await _paymentWebhookService
            .ProcessAsync(providerType, request, cancellationToken)
            .ConfigureAwait(false);

        return Ok(result);
    }

    /// <summary>
    /// Ingests payment-provider webhook events and applies idempotent processing.
    /// </summary>
    [HttpPost("webhooks/{providerType}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ProcessPaymentWebhookResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> IngestWebhook(
        [FromRoute] PaymentProviderType providerType,
        [FromBody] JsonElement requestBody,
        CancellationToken cancellationToken = default)
    {
        if (providerType == PaymentProviderType.ZaloPay)
        {
            return await IngestZaloPayWebhookAsync(requestBody, cancellationToken).ConfigureAwait(false);
        }

        try
        {
            var request = JsonSerializer.Deserialize<ProcessPaymentWebhookRequest>(requestBody.GetRawText())
                ?? throw new InvalidOperationException("Webhook request body is required.");

            await _processWebhookValidator
                .ValidateAndThrowAsync(request, cancellationToken)
                .ConfigureAwait(false);

            var response = await _paymentWebhookService
                .ProcessAsync(providerType, request, cancellationToken)
                .ConfigureAwait(false);

            return Ok(response);
        }
        catch (PaymentWebhookSignatureValidationException ex)
        {
            _logger.LogWarning(ex, "Webhook signature validation failed: providerType={ProviderType}", providerType);

            return Unauthorized(new ErrorResponse
            {
                Message = ex.Message,
                ErrorCode = "INVALID_SIGNATURE",
                Timestamp = DateTime.UtcNow
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid payment webhook payload/provider: providerType={ProviderType}", providerType);

            return BadRequest(new ErrorResponse
            {
                Message = ex.Message,
                ErrorCode = "INVALID_OPERATION",
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in IngestWebhook: providerType={ProviderType}", providerType);
            throw;
        }
    }

    private async Task<IActionResult> IngestZaloPayWebhookAsync(
        JsonElement requestBody,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = await _zaloPayWebhookIngressService
                .NormalizeAsync(requestBody, cancellationToken)
                .ConfigureAwait(false);

            await _processWebhookValidator
                .ValidateAndThrowAsync(request, cancellationToken)
                .ConfigureAwait(false);

            var response = await _paymentWebhookService
                .ProcessAsync(PaymentProviderType.ZaloPay, request, cancellationToken)
                .ConfigureAwait(false);

            return Ok(new
            {
                return_code = response.IsDuplicate ? 2 : 1,
                return_message = response.IsDuplicate ? "duplicate" : "success"
            });
        }
        catch (PaymentWebhookSignatureValidationException ex)
        {
            _logger.LogWarning(ex, "ZaloPay webhook signature validation failed.");

            return Ok(new
            {
                return_code = -1,
                return_message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ZaloPay webhook processing failed.");

            return Ok(new
            {
                return_code = 0,
                return_message = "temporary error"
            });
        }
    }

    private string ComputeMockSignature(PaymentProviderType providerType, ProcessPaymentWebhookRequest request)
    {
        if (!_webhookSecurityOptions.Enabled || !_webhookSecurityOptions.RequireSignature)
        {
            return "mock-signature-disabled";
        }

        if (string.IsNullOrWhiteSpace(_webhookSecurityOptions.SharedSecret))
        {
            throw new InvalidOperationException("Payment webhook signature secret is not configured.");
        }

        var canonicalMessage = PaymentWebhookSignatureHelper.BuildCanonicalMessage(providerType, request);
        return PaymentWebhookSignatureHelper.ComputeSignature(_webhookSecurityOptions.SharedSecret, canonicalMessage);
    }
}