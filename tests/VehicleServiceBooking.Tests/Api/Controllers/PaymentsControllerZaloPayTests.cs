using System.Text.Json;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using VehicleServiceBooking.Api.Controllers;
using VehicleServiceBooking.Application.Configuration;
using VehicleServiceBooking.Application.DTOs;
using VehicleServiceBooking.Application.Exceptions;
using VehicleServiceBooking.Application.Interfaces.Services;
using VehicleServiceBooking.Domain.Enums;

namespace VehicleServiceBooking.Tests.Api.Controllers;

public class PaymentsControllerZaloPayTests
{
    [Fact]
    public async Task IngestWebhook_WhenZaloPayPayloadIsValid_ShouldReturnProviderAck()
    {
        using var payload = JsonDocument.Parse("{\"data\":\"{}\",\"mac\":\"abc\",\"type\":1}");
        var normalizedRequest = new ProcessPaymentWebhookRequest
        {
            EventId = "zalopay-123",
            IntentCode = "PI-123",
            TransactionStatus = "Completed",
            SignatureHash = "internal-signature"
        };

        var validator = new Mock<IValidator<ProcessPaymentWebhookRequest>>();
        validator
            .Setup(x => x.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var webhookService = new Mock<IPaymentWebhookService>();
        webhookService
            .Setup(x => x.ProcessAsync(PaymentProviderType.ZaloPay, normalizedRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProcessPaymentWebhookResponse
            {
                IsDuplicate = false,
                ProcessStatus = "Processed",
                Message = "Webhook processed successfully."
            });

        var ingressService = new Mock<IZaloPayWebhookIngressService>();
        ingressService
            .Setup(x => x.NormalizeAsync(payload.RootElement, It.IsAny<CancellationToken>()))
            .ReturnsAsync(normalizedRequest);

        var controller = CreateController(validator.Object, webhookService.Object, ingressService.Object);

        var result = await controller.IngestWebhook(PaymentProviderType.ZaloPay, payload.RootElement, CancellationToken.None);

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        var json = JsonSerializer.Serialize(ok.Value);
        json.Should().Contain("\"return_code\":1");
        json.Should().Contain("\"return_message\":\"success\"");
    }

    [Fact]
    public async Task IngestWebhook_WhenZaloPaySignatureIsInvalid_ShouldReturnProviderInvalidMacAck()
    {
        using var payload = JsonDocument.Parse("{\"data\":\"{}\",\"mac\":\"abc\",\"type\":1}");

        var validator = new Mock<IValidator<ProcessPaymentWebhookRequest>>();
        var webhookService = new Mock<IPaymentWebhookService>();
        var ingressService = new Mock<IZaloPayWebhookIngressService>();
        ingressService
            .Setup(x => x.NormalizeAsync(payload.RootElement, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new PaymentWebhookSignatureValidationException("Webhook signature is invalid."));

        var controller = CreateController(validator.Object, webhookService.Object, ingressService.Object);

        var result = await controller.IngestWebhook(PaymentProviderType.ZaloPay, payload.RootElement, CancellationToken.None);

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        var json = JsonSerializer.Serialize(ok.Value);
        json.Should().Contain("\"return_code\":-1");
    }

    private static PaymentsController CreateController(
        IValidator<ProcessPaymentWebhookRequest> validator,
        IPaymentWebhookService webhookService,
        IZaloPayWebhookIngressService ingressService)
    {
        return new PaymentsController(
            validator,
            webhookService,
            ingressService,
            Options.Create(new PaymentWebhookSecurityOptions
            {
                Enabled = true,
                RequireSignature = true,
                SharedSecret = "internal-shared-secret"
            }),
            Mock.Of<ILogger<PaymentsController>>());
    }
}