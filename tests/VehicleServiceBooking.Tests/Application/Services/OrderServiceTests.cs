using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using VehicleServiceBooking.Application.DTOs;
using VehicleServiceBooking.Application.Exceptions;
using VehicleServiceBooking.Application.Interfaces.Repositories;
using VehicleServiceBooking.Application.Services;
using VehicleServiceBooking.Domain.Entities;

namespace VehicleServiceBooking.Tests.Application.Services;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _orderRepository = new();
    private readonly Mock<IServiceTypeRepository> _serviceTypeRepository = new();
    private readonly Mock<IAppointmentRepository> _appointmentRepository = new();
    private readonly Mock<ICurrencyLookupRepository> _currencyLookupRepository = new();
    private readonly Mock<ILogger<OrderService>> _logger = new();

    [Fact]
    public async Task GetOrderByIdAsync_WhenOrderExists_ShouldReturnMappedResponse()
    {
        var orderId = Guid.NewGuid();
        var serviceTypeId = Guid.NewGuid();
        var appointmentId = Guid.NewGuid();
        var currencyId = Guid.Parse("00000000-0000-0000-0004-000000000001");
        var createdAt = DateTime.UtcNow;

        _orderRepository
            .Setup(x => x.GetAggregateByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Order
            {
                Id = orderId,
                OrderCode = "ORD-GET-001",
                CurrencyId = currencyId,
                TotalAmount = 200m,
                CreatedAt = createdAt,
                ServiceTypeOrders =
                [
                    new ServiceTypeOrder
                    {
                        ServiceTypeId = serviceTypeId,
                        Quantity = 2,
                        UnitPrice = 100m,
                        LineTotal = 200m
                    }
                ],
                OrderAppointments =
                [
                    new OrderAppointment
                    {
                        AppointmentId = appointmentId
                    }
                ]
            });

        var service = CreateService();

        var result = await service.GetOrderByIdAsync(orderId, CancellationToken.None);

        result.Should().NotBeNull();
        result!.OrderId.Should().Be(orderId);
        result.OrderCode.Should().Be("ORD-GET-001");
        result.CurrencyId.Should().Be(currencyId);
        result.TotalAmount.Should().Be(200m);
        result.CreatedAt.Should().Be(createdAt);
        result.ServiceTypeItems.Should().ContainSingle();
        result.ServiceTypeItems.Single().ServiceTypeId.Should().Be(serviceTypeId);
        result.AppointmentIds.Should().ContainSingle().Which.Should().Be(appointmentId);
    }

    [Fact]
    public async Task GetOrderByIdAsync_WhenOrderDoesNotExist_ShouldReturnNull()
    {
        var orderId = Guid.NewGuid();

        _orderRepository
            .Setup(x => x.GetAggregateByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        var service = CreateService();

        var result = await service.GetOrderByIdAsync(orderId, CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateOrderAsync_WithValidRequest_ShouldReturnComputedTotal()
    {
        var serviceTypeId = Guid.NewGuid();
        var appointmentId = Guid.NewGuid();
        var currencyId = Guid.Parse("00000000-0000-0000-0004-000000000001");
        var request = new CreateOrderRequest
        {
            OrderCode = "ORD-001",
            CurrencyId = currencyId,
            ServiceTypeItems =
            [
                new CreateOrderServiceTypeItemRequest { ServiceTypeId = serviceTypeId, Quantity = 3 }
            ],
            AppointmentIds = [appointmentId]
        };

        Order? persistedOrder = null;
        IReadOnlyCollection<ServiceTypeOrder>? persistedServiceTypeOrders = null;

        _currencyLookupRepository
            .Setup(x => x.GetByIdAsync(currencyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CurrencyLookup
            {
                Id = currencyId,
                Code = "VND",
                DecimalPlaces = 0,
                IsActive = true
            });

        _orderRepository
            .Setup(x => x.OrderCodeExistsAsync("ORD-001", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _serviceTypeRepository
            .Setup(x => x.GetByIdsWithPricesAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ServiceType>
            {
                new()
                {
                    Id = serviceTypeId,
                    ServiceTypePrices =
                    [
                        new ServiceTypePrice
                        {
                            Id = Guid.NewGuid(),
                            ServiceTypeId = serviceTypeId,
                            CurrencyId = currencyId,
                            Price = 100m,
                            IsActive = true
                        }
                    ]
                }
            });

        _appointmentRepository
            .Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Appointment>
            {
                new() { Id = appointmentId }
            });

        _orderRepository
            .Setup(x => x.AnyAppointmentAlreadyLinkedAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _orderRepository
            .Setup(x => x.CreateAggregateAsync(
                It.IsAny<Order>(),
                It.IsAny<IReadOnlyCollection<ServiceTypeOrder>>(),
                It.IsAny<IReadOnlyCollection<OrderAppointment>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order order, IReadOnlyCollection<ServiceTypeOrder> serviceTypeOrders, IReadOnlyCollection<OrderAppointment> _, CancellationToken _) =>
            {
                persistedOrder = order;
                persistedServiceTypeOrders = serviceTypeOrders;
                return order;
            });

        var service = CreateService();

        var result = await service.CreateOrderAsync(request, CancellationToken.None);

        result.OrderCode.Should().Be("ORD-001");
        result.CurrencyId.Should().Be(currencyId);
        result.TotalAmount.Should().Be(300m);
        result.ServiceTypeItems.Should().ContainSingle();
        result.AppointmentIds.Should().ContainSingle().Which.Should().Be(appointmentId);
        persistedOrder.Should().NotBeNull();
        persistedOrder!.CurrencyId.Should().Be(currencyId);
        persistedOrder.TotalAmount.Should().Be(300m);
        persistedServiceTypeOrders.Should().NotBeNull();
        var persistedServiceTypeOrder = persistedServiceTypeOrders!.Single();
        persistedServiceTypeOrder.UnitPrice.Should().Be(100m);
        persistedServiceTypeOrder.LineTotal.Should().Be(300m);
    }

    [Fact]
    public async Task CreateOrderAsync_WithDuplicateOrderCode_ShouldThrowOrderConflictException()
    {
        var currencyId = Guid.Parse("00000000-0000-0000-0004-000000000001");
        var request = new CreateOrderRequest
        {
            OrderCode = "ORD-001",
            CurrencyId = currencyId,
            ServiceTypeItems =
            [
                new CreateOrderServiceTypeItemRequest { ServiceTypeId = Guid.NewGuid(), Quantity = 1 }
            ],
            AppointmentIds = []
        };

        _currencyLookupRepository
            .Setup(x => x.GetByIdAsync(currencyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CurrencyLookup
            {
                Id = currencyId,
                Code = "VND",
                DecimalPlaces = 0,
                IsActive = true
            });

        _orderRepository
            .Setup(x => x.OrderCodeExistsAsync("ORD-001", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var service = CreateService();

        var action = async () => await service.CreateOrderAsync(request, CancellationToken.None);

        await action.Should().ThrowAsync<OrderConflictException>();
    }

    [Fact]
    public async Task CreateOrderAsync_WithNoActiveServiceTypePrice_ShouldThrowInvalidOperationException()
    {
        var serviceTypeId = Guid.NewGuid();
        var currencyId = Guid.Parse("00000000-0000-0000-0004-000000000001");
        var request = new CreateOrderRequest
        {
            OrderCode = "ORD-001",
            CurrencyId = currencyId,
            ServiceTypeItems =
            [
                new CreateOrderServiceTypeItemRequest { ServiceTypeId = serviceTypeId, Quantity = 1 }
            ],
            AppointmentIds = []
        };

        _currencyLookupRepository
            .Setup(x => x.GetByIdAsync(currencyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CurrencyLookup
            {
                Id = currencyId,
                Code = "VND",
                DecimalPlaces = 0,
                IsActive = true
            });

        _orderRepository
            .Setup(x => x.OrderCodeExistsAsync("ORD-001", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _serviceTypeRepository
            .Setup(x => x.GetByIdsWithPricesAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ServiceType>
            {
                new()
                {
                    Id = serviceTypeId,
                    ServiceTypePrices = []
                }
            });

        _orderRepository
            .Setup(x => x.CreateAggregateAsync(
                It.IsAny<Order>(),
                It.IsAny<IReadOnlyCollection<ServiceTypeOrder>>(),
                It.IsAny<IReadOnlyCollection<OrderAppointment>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order order, IReadOnlyCollection<ServiceTypeOrder> _, IReadOnlyCollection<OrderAppointment> _, CancellationToken _) => order);

        var service = CreateService();

        var action = async () => await service.CreateOrderAsync(request, CancellationToken.None);

        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("No active ServiceTypePrice is configured for service type * and currency *");
    }

    [Fact]
    public async Task CreateOrderAsync_ShouldRoundFinalTotalUsingCurrencyDecimalPlaces()
    {
        var serviceTypeId = Guid.NewGuid();
        var currencyId = Guid.Parse("00000000-0000-0000-0004-000000000001");
        var request = new CreateOrderRequest
        {
            OrderCode = "ORD-ROUND-001",
            CurrencyId = currencyId,
            ServiceTypeItems =
            [
                new CreateOrderServiceTypeItemRequest { ServiceTypeId = serviceTypeId, Quantity = 3 }
            ],
            AppointmentIds = []
        };

        _currencyLookupRepository
            .Setup(x => x.GetByIdAsync(currencyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CurrencyLookup
            {
                Id = currencyId,
                Code = "VND",
                DecimalPlaces = 0,
                IsActive = true
            });

        _orderRepository
            .Setup(x => x.OrderCodeExistsAsync("ORD-ROUND-001", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _serviceTypeRepository
            .Setup(x => x.GetByIdsWithPricesAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ServiceType>
            {
                new()
                {
                    Id = serviceTypeId,
                    ServiceTypePrices =
                    [
                        new ServiceTypePrice
                        {
                            Id = Guid.NewGuid(),
                            ServiceTypeId = serviceTypeId,
                            CurrencyId = currencyId,
                            Price = 33.6m,
                            IsActive = true
                        }
                    ]
                }
            });

        _orderRepository
            .Setup(x => x.CreateAggregateAsync(
                It.IsAny<Order>(),
                It.IsAny<IReadOnlyCollection<ServiceTypeOrder>>(),
                It.IsAny<IReadOnlyCollection<OrderAppointment>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order order, IReadOnlyCollection<ServiceTypeOrder> _, IReadOnlyCollection<OrderAppointment> _, CancellationToken _) => order);

        var service = CreateService();

        var result = await service.CreateOrderAsync(request, CancellationToken.None);

        result.TotalAmount.Should().Be(101m);
    }

    [Fact]
    public async Task CreateOrderAsync_WithInvalidCurrency_ShouldThrowInvalidOperationException()
    {
        var request = new CreateOrderRequest
        {
            OrderCode = "ORD-001",
            CurrencyId = Guid.NewGuid(),
            ServiceTypeItems =
            [
                new CreateOrderServiceTypeItemRequest { ServiceTypeId = Guid.NewGuid(), Quantity = 1 }
            ],
            AppointmentIds = []
        };

        _currencyLookupRepository
            .Setup(x => x.GetByIdAsync(request.CurrencyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CurrencyLookup?)null);

        var service = CreateService();

        var action = async () => await service.CreateOrderAsync(request, CancellationToken.None);

        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Currency '{request.CurrencyId}' was not found or is inactive.");
    }

    private OrderService CreateService()
    {
        return new OrderService(
            _orderRepository.Object,
            _serviceTypeRepository.Object,
            _appointmentRepository.Object,
            _currencyLookupRepository.Object,
            _logger.Object);
    }
}
