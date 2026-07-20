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
using VehicleServiceBooking.Application.Interfaces.Services;

namespace VehicleServiceBooking.Tests.Api.Controllers;

public class OrdersControllerTests
{
    [Fact]
    public async Task GetOrderById_WhenOrderExists_ReturnsOk()
    {
        var orderId = Guid.NewGuid();
        var response = CreateSuccessResponse();

        var validator = new Mock<IValidator<CreateOrderRequest>>();
        var orderService = new Mock<IOrderService>();
        orderService
            .Setup(s => s.GetOrderByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var idempotencyService = new Mock<IIdempotencyService>();
        var idempotencyCoordinator = new Mock<IIdempotencyRequestCoordinator>();
        var logger = new Mock<ILogger<OrdersController>>();

        var controller = new OrdersController(
            validator.Object,
            orderService.Object,
            idempotencyService.Object,
            idempotencyCoordinator.Object,
            logger.Object);

        var actionResult = await controller.GetOrderById(orderId, CancellationToken.None);

        var okResult = actionResult.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task GetOrderById_WhenOrderNotFound_ReturnsNotFound()
    {
        var orderId = Guid.NewGuid();

        var validator = new Mock<IValidator<CreateOrderRequest>>();
        var orderService = new Mock<IOrderService>();
        orderService
            .Setup(s => s.GetOrderByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CreateOrderResponse?)null);

        var idempotencyService = new Mock<IIdempotencyService>();
        var idempotencyCoordinator = new Mock<IIdempotencyRequestCoordinator>();
        var logger = new Mock<ILogger<OrdersController>>();

        var controller = new OrdersController(
            validator.Object,
            orderService.Object,
            idempotencyService.Object,
            idempotencyCoordinator.Object,
            logger.Object);

        var actionResult = await controller.GetOrderById(orderId, CancellationToken.None);

        var notFoundResult = actionResult.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        var error = notFoundResult.Value.Should().BeOfType<ErrorResponse>().Subject;

        error.ErrorCode.Should().Be("ORDER_NOT_FOUND");
    }

    [Fact]
    public async Task CreateOrder_WhenOrderCodeMissing_GeneratesOrderCodeBeforeValidation()
    {
        var request = CreateValidRequest();
        request.OrderCode = string.Empty;

        var validator = new Mock<IValidator<CreateOrderRequest>>();
        validator
            .Setup(v => v.ValidateAsync(
                It.Is<ValidationContext<CreateOrderRequest>>(ctx => !string.IsNullOrWhiteSpace(ctx.InstanceToValidate.OrderCode)),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var orderService = new Mock<IOrderService>();
        orderService
            .Setup(s => s.CreateOrderAsync(
                It.Is<CreateOrderRequest>(r => !string.IsNullOrWhiteSpace(r.OrderCode)),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateSuccessResponse());

        var idempotencyService = new Mock<IIdempotencyService>();
        var idempotencyCoordinator = new Mock<IIdempotencyRequestCoordinator>();
        idempotencyCoordinator
            .Setup(c => c.ValidateAndBeginCreateOrderAsync(It.IsAny<HttpRequest>(), It.IsAny<CreateOrderRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new IdempotencyCoordinatorResult());

        var logger = new Mock<ILogger<OrdersController>>();
        var controller = new OrdersController(
            validator.Object,
            orderService.Object,
            idempotencyService.Object,
            idempotencyCoordinator.Object,
            logger.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        var actionResult = await controller.CreateOrder(request, null, CancellationToken.None);

        actionResult.Result.Should().BeOfType<CreatedResult>();
        validator.Verify(v => v.ValidateAsync(
            It.Is<ValidationContext<CreateOrderRequest>>(ctx => !string.IsNullOrWhiteSpace(ctx.InstanceToValidate.OrderCode)),
            It.IsAny<CancellationToken>()), Times.Once);
        orderService.Verify(s => s.CreateOrderAsync(
            It.Is<CreateOrderRequest>(r => !string.IsNullOrWhiteSpace(r.OrderCode)),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateOrder_WhenIdempotencyHeaderMissing_CreatesOrderWithoutTracking()
    {
        var request = CreateValidRequest();

        var validator = new Mock<IValidator<CreateOrderRequest>>();
        validator
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<CreateOrderRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var orderService = new Mock<IOrderService>();
        orderService
            .Setup(s => s.CreateOrderAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateSuccessResponse());

        var idempotencyService = new Mock<IIdempotencyService>(MockBehavior.Strict);

        var idempotencyCoordinator = new Mock<IIdempotencyRequestCoordinator>();
        idempotencyCoordinator
            .Setup(c => c.ValidateAndBeginCreateOrderAsync(
                It.Is<HttpRequest>(r => !r.Headers.ContainsKey("Idempotency-Key")),
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new IdempotencyCoordinatorResult());

        var logger = new Mock<ILogger<OrdersController>>();
        var controller = new OrdersController(
            validator.Object,
            orderService.Object,
            idempotencyService.Object,
            idempotencyCoordinator.Object,
            logger.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        var actionResult = await controller.CreateOrder(request, null, CancellationToken.None);

        actionResult.Result.Should().BeOfType<CreatedResult>();
        idempotencyService.Verify(
            s => s.CompleteRequestAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateOrder_WhenIdempotencyHeaderPresent_CreatesOrderAndTracksResult()
    {
        var request = CreateValidRequest();
        var recordId = Guid.NewGuid();

        var validator = new Mock<IValidator<CreateOrderRequest>>();
        validator
            .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var orderService = new Mock<IOrderService>();
        orderService
            .Setup(s => s.CreateOrderAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateSuccessResponse());

        var idempotencyService = new Mock<IIdempotencyService>();
        idempotencyService
            .Setup(s => s.CompleteRequestAsync(
                recordId,
                StatusCodes.Status201Created,
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var idempotencyCoordinator = new Mock<IIdempotencyRequestCoordinator>();
        idempotencyCoordinator
            .Setup(c => c.ValidateAndBeginCreateOrderAsync(
                It.Is<HttpRequest>(r => r.Headers.ContainsKey("Idempotency-Key")),
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new IdempotencyCoordinatorResult
            {
                RecordId = recordId
            });

        var logger = new Mock<ILogger<OrdersController>>();
        var controller = new OrdersController(
            validator.Object,
            orderService.Object,
            idempotencyService.Object,
            idempotencyCoordinator.Object,
            logger.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        controller.ControllerContext.HttpContext.Request.Headers["Idempotency-Key"] = "create-order-test-key";

        var actionResult = await controller.CreateOrder(request, "create-order-test-key", CancellationToken.None);

        actionResult.Result.Should().BeOfType<CreatedResult>();
        idempotencyService.Verify(
            s => s.CompleteRequestAsync(
                recordId,
                StatusCodes.Status201Created,
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateOrder_WhenIdempotencyKeyReusedWithDifferentPayload_ReturnsConflict()
    {
        var request = CreateValidRequest();

        var validator = new Mock<IValidator<CreateOrderRequest>>();
        validator
            .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var orderService = new Mock<IOrderService>();
        var idempotencyService = new Mock<IIdempotencyService>();

        var idempotencyCoordinator = new Mock<IIdempotencyRequestCoordinator>();
        idempotencyCoordinator
            .Setup(c => c.ValidateAndBeginCreateOrderAsync(It.IsAny<HttpRequest>(), request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new IdempotencyCoordinatorResult
            {
                EarlyResponse = new ConflictObjectResult(new ErrorResponse
                {
                    Message = "The provided idempotency key was already used with a different request payload.",
                    ErrorCode = "IDEMPOTENCY_KEY_REUSED",
                    Timestamp = DateTime.UtcNow
                })
            });

        var logger = new Mock<ILogger<OrdersController>>();
        var controller = new OrdersController(
            validator.Object,
            orderService.Object,
            idempotencyService.Object,
            idempotencyCoordinator.Object,
            logger.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        controller.ControllerContext.HttpContext.Request.Headers["Idempotency-Key"] = "duplicate-key";

        var actionResult = await controller.CreateOrder(request, null, CancellationToken.None);

        var conflictResult = actionResult.Result.Should().BeOfType<ConflictObjectResult>().Subject;
        var error = conflictResult.Value.Should().BeOfType<ErrorResponse>().Subject;

        error.ErrorCode.Should().Be("IDEMPOTENCY_KEY_REUSED");
        orderService.Verify(s => s.CreateOrderAsync(It.IsAny<CreateOrderRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateOrder_WhenOrderConflictExceptionThrown_ReturnsConflict()
    {
        var request = CreateValidRequest();
        var recordId = Guid.NewGuid();

        var validator = new Mock<IValidator<CreateOrderRequest>>();
        validator
            .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var orderService = new Mock<IOrderService>();
        orderService
            .Setup(s => s.CreateOrderAsync(request, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OrderConflictException("Duplicate order code"));

        var idempotencyService = new Mock<IIdempotencyService>();
        idempotencyService
            .Setup(s => s.CompleteRequestAsync(
                recordId,
                StatusCodes.Status409Conflict,
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var idempotencyCoordinator = new Mock<IIdempotencyRequestCoordinator>();
        idempotencyCoordinator
            .Setup(c => c.ValidateAndBeginCreateOrderAsync(It.IsAny<HttpRequest>(), request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new IdempotencyCoordinatorResult { RecordId = recordId });

        var logger = new Mock<ILogger<OrdersController>>();
        var controller = new OrdersController(
            validator.Object,
            orderService.Object,
            idempotencyService.Object,
            idempotencyCoordinator.Object,
            logger.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        var actionResult = await controller.CreateOrder(request, null, CancellationToken.None);

        var conflictResult = actionResult.Result.Should().BeOfType<ConflictObjectResult>().Subject;
        var error = conflictResult.Value.Should().BeOfType<ErrorResponse>().Subject;

        error.ErrorCode.Should().Be("ORDER_CONFLICT");
    }

    private static CreateOrderRequest CreateValidRequest()
    {
        return new CreateOrderRequest
        {
            OrderCode = "ORD-2026-1001",
            CurrencyId = Guid.Parse("00000000-0000-0000-0004-000000000001"),
            ServiceTypeItems =
            [
                new CreateOrderServiceTypeItemRequest
                {
                    ServiceTypeId = Guid.NewGuid(),
                    Quantity = 1
                }
            ],
            AppointmentIds = [Guid.NewGuid()]
        };
    }

    private static CreateOrderResponse CreateSuccessResponse()
    {
        return new CreateOrderResponse
        {
            OrderId = Guid.NewGuid(),
            OrderCode = "ORD-2026-1001",
            CurrencyId = Guid.Parse("00000000-0000-0000-0004-000000000001"),
            TotalAmount = 100m,
            CreatedAt = DateTime.UtcNow,
            ServiceTypeItems =
            [
                new CreateOrderServiceTypeItemResponse
                {
                    ServiceTypeId = Guid.NewGuid(),
                    Quantity = 1,
                    UnitPrice = 100m,
                    LineTotal = 100m
                }
            ],
            AppointmentIds = [Guid.NewGuid()]
        };
    }
}
