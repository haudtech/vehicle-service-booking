using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using VehicleServiceBooking.Api.Controllers;
using VehicleServiceBooking.Api.Services;
using VehicleServiceBooking.Application.DTOs;
using VehicleServiceBooking.Application.Exceptions;
using VehicleServiceBooking.Application.Interfaces;
using VehicleServiceBooking.Application.Interfaces.Services;
using VehicleServiceBooking.Domain.Enums;
using VehicleServiceBooking.Tests.Common;
using Xunit;

namespace VehicleServiceBooking.Tests.Api.Controllers;

public class AppointmentsControllerTests
{
    [Fact]
    public async Task CreateAppointment_WhenIdempotencyEnabledAndHeaderMissing_CreatesAppointmentWithoutTracking()
    {
        var request = new CreateAppointmentRequestBuilder().Build();

        var validator = new Mock<IValidator<CreateAppointmentRequest>>();
        validator
            .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var appointmentService = new Mock<IAppointmentService>();
        appointmentService
            .Setup(s => s.CreateAppointmentAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateAppointmentResponse
            {
                AppointmentId = Guid.NewGuid(),
                SlotStart = DateTime.UtcNow,
                SlotEnd = DateTime.UtcNow.AddHours(1),
                CreatedAt = DateTime.UtcNow
            });

        var idempotencyService = new Mock<IIdempotencyService>(MockBehavior.Strict);

        var idempotencyCoordinator = new Mock<IIdempotencyRequestCoordinator>();
        idempotencyCoordinator
            .Setup(c => c.ValidateAndBeginCreateAppointmentAsync(
                It.Is<HttpRequest>(r => !r.Headers.ContainsKey("Idempotency-Key")),
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new IdempotencyCoordinatorResult());

        var logger = new Mock<ILogger<AppointmentsController>>();
        var controller = new AppointmentsController(
            validator.Object,
            appointmentService.Object,
            idempotencyService.Object,
            idempotencyCoordinator.Object,
            logger.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        var actionResult = await controller.CreateAppointment(request, null, CancellationToken.None);

        actionResult.Result.Should().BeOfType<CreatedAtActionResult>();
        idempotencyService.Verify(
            s => s.CompleteRequestAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateAppointment_WhenIdempotencyEnabledAndHeaderPresent_CreatesAppointmentAndTracksResult()
    {
        var request = new CreateAppointmentRequestBuilder().Build();
        var recordId = Guid.NewGuid();

        var validator = new Mock<IValidator<CreateAppointmentRequest>>();
        validator
            .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var appointmentResponse = new CreateAppointmentResponse
        {
            AppointmentId = Guid.NewGuid(),
            SlotStart = DateTime.UtcNow,
            SlotEnd = DateTime.UtcNow.AddHours(1),
            CreatedAt = DateTime.UtcNow
        };

        var appointmentService = new Mock<IAppointmentService>();
        appointmentService
            .Setup(s => s.CreateAppointmentAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointmentResponse);

        var idempotencyService = new Mock<IIdempotencyService>();

        var idempotencyCoordinator = new Mock<IIdempotencyRequestCoordinator>();
        idempotencyCoordinator
            .Setup(c => c.ValidateAndBeginCreateAppointmentAsync(
                It.Is<HttpRequest>(r => r.Headers.ContainsKey("Idempotency-Key")),
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new IdempotencyCoordinatorResult
            {
                RecordId = recordId
            });

        idempotencyService
            .Setup(s => s.CompleteRequestAsync(
                recordId,
                StatusCodes.Status201Created,
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var logger = new Mock<ILogger<AppointmentsController>>();
        var controller = new AppointmentsController(
            validator.Object,
            appointmentService.Object,
            idempotencyService.Object,
            idempotencyCoordinator.Object,
            logger.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        controller.ControllerContext.HttpContext.Request.Headers["Idempotency-Key"] = "create-appointment-test-key";

        var actionResult = await controller.CreateAppointment(request, "create-appointment-test-key", CancellationToken.None);

        actionResult.Result.Should().BeOfType<CreatedAtActionResult>();
        idempotencyService.Verify(
            s => s.CompleteRequestAsync(
                recordId,
                StatusCodes.Status201Created,
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CancelAppointment_WhenAppointmentExists_ReturnsOkWithMessage()
    {
        var appointmentId = Guid.NewGuid();
        var response = new AppointmentStatusUpdateResponse
        {
            AppointmentId = appointmentId,
            Message = "Appointment cancelled successfully."
        };

        var validator = new Mock<IValidator<CreateAppointmentRequest>>();
        var appointmentService = new Mock<IAppointmentService>();
        appointmentService
            .Setup(s => s.CancelAppointmentAsync(appointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var idempotencyService = new Mock<IIdempotencyService>();
        var idempotencyCoordinator = new Mock<IIdempotencyRequestCoordinator>();
        var logger = new Mock<ILogger<AppointmentsController>>();
        var controller = new AppointmentsController(
            validator.Object,
            appointmentService.Object,
            idempotencyService.Object,
            idempotencyCoordinator.Object,
            logger.Object);

        var actionResult = await controller.CancelAppointment(appointmentId, CancellationToken.None);

        var okResult = actionResult.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task CreateAppointment_WhenBookingConflictExceptionThrown_ReturnsConflictWithStableErrorCode()
    {
        var request = new CreateAppointmentRequestBuilder().Build();

        var validator = new Mock<IValidator<CreateAppointmentRequest>>();
        validator
            .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var appointmentService = new Mock<IAppointmentService>();
        appointmentService
            .Setup(s => s.CreateAppointmentAsync(request, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new BookingConflictException("Slot already taken"));

        var idempotencyService = new Mock<IIdempotencyService>();

        var idempotencyCoordinator = new Mock<IIdempotencyRequestCoordinator>();
        idempotencyCoordinator
            .Setup(c => c.ValidateAndBeginCreateAppointmentAsync(It.IsAny<HttpRequest>(), request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new IdempotencyCoordinatorResult { RecordId = Guid.NewGuid() });

        var logger = new Mock<ILogger<AppointmentsController>>();
        var controller = new AppointmentsController(
            validator.Object,
            appointmentService.Object,
            idempotencyService.Object,
            idempotencyCoordinator.Object,
            logger.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        var actionResult = await controller.CreateAppointment(request, null, CancellationToken.None);

        var conflictResult = actionResult.Result.Should().BeOfType<ConflictObjectResult>().Subject;
        var error = conflictResult.Value.Should().BeOfType<ErrorResponse>().Subject;

        error.ErrorCode.Should().Be("BOOKING_CONFLICT");
        error.Message.Should().Be("The selected slot is no longer available. Please check availability again.");
    }

    [Fact]
    public async Task CreateAppointment_WhenIdempotencyKeyReusedWithDifferentPayload_ReturnsConflict()
    {
        var request = new CreateAppointmentRequestBuilder().Build();

        var validator = new Mock<IValidator<CreateAppointmentRequest>>();
        validator
            .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var appointmentService = new Mock<IAppointmentService>();

        var idempotencyService = new Mock<IIdempotencyService>();

        var idempotencyCoordinator = new Mock<IIdempotencyRequestCoordinator>();
        idempotencyCoordinator
            .Setup(c => c.ValidateAndBeginCreateAppointmentAsync(It.IsAny<HttpRequest>(), request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new IdempotencyCoordinatorResult
            {
                EarlyResponse = new ConflictObjectResult(new ErrorResponse
                {
                    Message = "The provided idempotency key was already used with a different request payload.",
                    ErrorCode = "IDEMPOTENCY_KEY_REUSED",
                    Timestamp = DateTime.UtcNow
                })
            });

        var logger = new Mock<ILogger<AppointmentsController>>();
        var controller = new AppointmentsController(
            validator.Object,
            appointmentService.Object,
            idempotencyService.Object,
            idempotencyCoordinator.Object,
            logger.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        controller.ControllerContext.HttpContext.Request.Headers["Idempotency-Key"] = "duplicate-key";

        var actionResult = await controller.CreateAppointment(request, null, CancellationToken.None);

        var conflictResult = actionResult.Result.Should().BeOfType<ConflictObjectResult>().Subject;
        var error = conflictResult.Value.Should().BeOfType<ErrorResponse>().Subject;

        error.ErrorCode.Should().Be("IDEMPOTENCY_KEY_REUSED");
        appointmentService.Verify(s => s.CreateAppointmentAsync(It.IsAny<CreateAppointmentRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAppointment_WhenIdempotencyReplayExists_ReturnsStoredResponseWithoutCallingService()
    {
        var request = new CreateAppointmentRequestBuilder().Build();

        var validator = new Mock<IValidator<CreateAppointmentRequest>>();
        validator
            .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var appointmentService = new Mock<IAppointmentService>();

        var stored = new CreateAppointmentResponse
        {
            AppointmentId = Guid.NewGuid(),
            SlotStart = DateTime.UtcNow,
            SlotEnd = DateTime.UtcNow.AddHours(1),
            CreatedAt = DateTime.UtcNow
        };

        var idempotencyService = new Mock<IIdempotencyService>();

        var idempotencyCoordinator = new Mock<IIdempotencyRequestCoordinator>();
        idempotencyCoordinator
            .Setup(c => c.ValidateAndBeginCreateAppointmentAsync(It.IsAny<HttpRequest>(), request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new IdempotencyCoordinatorResult
            {
                EarlyResponse = new ContentResult
                {
                    StatusCode = StatusCodes.Status201Created,
                    ContentType = "application/json",
                    Content = System.Text.Json.JsonSerializer.Serialize(stored)
                }
            });

        var logger = new Mock<ILogger<AppointmentsController>>();
        var controller = new AppointmentsController(
            validator.Object,
            appointmentService.Object,
            idempotencyService.Object,
            idempotencyCoordinator.Object,
            logger.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        controller.ControllerContext.HttpContext.Request.Headers["Idempotency-Key"] = "replay-key";

        var actionResult = await controller.CreateAppointment(request, null, CancellationToken.None);

        var contentResult = actionResult.Result.Should().BeOfType<ContentResult>().Subject;
        contentResult.StatusCode.Should().Be(StatusCodes.Status201Created);
        contentResult.Content.Should().Contain(stored.AppointmentId.ToString());
        appointmentService.Verify(s => s.CreateAppointmentAsync(It.IsAny<CreateAppointmentRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
