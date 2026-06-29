using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using VehicleServiceBooking.Application.Configuration;
using VehicleServiceBooking.Application.DTOs;
using VehicleServiceBooking.Application.Interfaces.Services;

namespace VehicleServiceBooking.Api.Services;

/// <summary>
/// Handles API-specific idempotency header validation and begin/replay decision flow.
/// </summary>
public sealed class IdempotencyRequestCoordinator : IIdempotencyRequestCoordinator
{
    private const string IdempotencyKeyHeaderName = "Idempotency-Key";

    private readonly IIdempotencyService _idempotencyService;
    private readonly IdempotencyOptions _idempotencyOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="IdempotencyRequestCoordinator"/> class.
    /// </summary>
    public IdempotencyRequestCoordinator(
        IIdempotencyService idempotencyService,
        IOptions<IdempotencyOptions> idempotencyOptions)
    {
        _idempotencyService = idempotencyService ?? throw new ArgumentNullException(nameof(idempotencyService));
        _idempotencyOptions = idempotencyOptions?.Value ?? throw new ArgumentNullException(nameof(idempotencyOptions));
    }

    /// <inheritdoc />
    public async Task<IdempotencyCoordinatorResult> ValidateAndBeginCreateAppointmentAsync(
        HttpRequest httpRequest,
        CreateAppointmentRequest request,
        CancellationToken cancellationToken)
    {
        if (!_idempotencyOptions.Enabled)
        {
            return new IdempotencyCoordinatorResult();
        }

        if (!httpRequest.Headers.TryGetValue(IdempotencyKeyHeaderName, out var idempotencyHeaderValues) ||
            string.IsNullOrWhiteSpace(idempotencyHeaderValues.ToString()))
        {
            if (_idempotencyOptions.RequireHeader)
            {
                return new IdempotencyCoordinatorResult
                {
                    EarlyResponse = new BadRequestObjectResult(new ErrorResponse
                    {
                        Message = $"Missing required header '{IdempotencyKeyHeaderName}'.",
                        ErrorCode = "IDEMPOTENCY_KEY_REQUIRED",
                        Timestamp = DateTime.UtcNow
                    })
                };
            }

            return new IdempotencyCoordinatorResult();
        }

        var idempotencyKey = idempotencyHeaderValues.ToString().Trim();
        if (idempotencyKey.Length > _idempotencyOptions.KeyMaxLength)
        {
            return new IdempotencyCoordinatorResult
            {
                EarlyResponse = new BadRequestObjectResult(new ErrorResponse
                {
                    Message = $"Header '{IdempotencyKeyHeaderName}' exceeds max length {_idempotencyOptions.KeyMaxLength}.",
                    ErrorCode = "IDEMPOTENCY_KEY_INVALID",
                    Timestamp = DateTime.UtcNow
                })
            };
        }

        var requestPath = $"POST:{httpRequest.Path.Value ?? "/api/v1/appointments"}";
        var requestHash = ComputeRequestHash(request);

        var beginResult = await _idempotencyService
            .BeginRequestAsync(idempotencyKey, requestPath, requestHash, cancellationToken)
            .ConfigureAwait(false);

        if (beginResult.Outcome == IdempotencyBeginOutcome.KeyReusedWithDifferentPayload)
        {
            return new IdempotencyCoordinatorResult
            {
                EarlyResponse = new ConflictObjectResult(new ErrorResponse
                {
                    Message = "The provided idempotency key was already used with a different request payload.",
                    ErrorCode = "IDEMPOTENCY_KEY_REUSED",
                    Timestamp = DateTime.UtcNow
                })
            };
        }

        if (beginResult.Outcome == IdempotencyBeginOutcome.RequestInProgress)
        {
            return new IdempotencyCoordinatorResult
            {
                EarlyResponse = new ConflictObjectResult(new ErrorResponse
                {
                    Message = "A request with this idempotency key is already in progress.",
                    ErrorCode = "IDEMPOTENCY_IN_PROGRESS",
                    Timestamp = DateTime.UtcNow
                })
            };
        }

        if (beginResult.Outcome == IdempotencyBeginOutcome.ReplayStoredResponse &&
            beginResult.ResponseStatusCode.HasValue &&
            !string.IsNullOrWhiteSpace(beginResult.ResponseBody))
        {
            return new IdempotencyCoordinatorResult
            {
                EarlyResponse = new ContentResult
                {
                    StatusCode = beginResult.ResponseStatusCode,
                    ContentType = "application/json",
                    Content = beginResult.ResponseBody
                }
            };
        }

        return new IdempotencyCoordinatorResult
        {
            RecordId = beginResult.RecordId
        };
    }

    private static string ComputeRequestHash(CreateAppointmentRequest request)
    {
        var canonicalJson = JsonSerializer.Serialize(request);
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(canonicalJson));
        return Convert.ToHexString(hashBytes);
    }
}