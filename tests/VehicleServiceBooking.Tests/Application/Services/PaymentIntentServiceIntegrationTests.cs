using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using VehicleServiceBooking.Application.DTOs;
using VehicleServiceBooking.Application.Services;
using VehicleServiceBooking.Domain.Entities;
using VehicleServiceBooking.Domain.Enums;
using VehicleServiceBooking.Infrastructure.Persistence;
using VehicleServiceBooking.Infrastructure.Repositories;

namespace VehicleServiceBooking.Tests.Application.Services;

public class PaymentIntentServiceIntegrationTests : IAsyncLifetime
{
    private ApplicationDbContext _dbContext = null!;
    private PaymentIntentService _service = null!;

    public async Task InitializeAsync()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"PaymentIntentServiceIntegrationTests_{Guid.NewGuid()}")
            .Options;

        _dbContext = new ApplicationDbContext(options);
        await _dbContext.Database.EnsureCreatedAsync();

        var repository = new PaymentIntentRepository(_dbContext);
        _service = new PaymentIntentService(
            repository,
            new DevelopmentPaymentProviderGateway(),
            Mock.Of<ILogger<PaymentIntentService>>());
    }

    public async Task DisposeAsync()
    {
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.DisposeAsync();
    }

    [Fact]
    public async Task CreatePaymentIntentAsync_WhenNoOpenIntent_ShouldCreateNewIntentAndTransaction()
    {
        var paymentProviderId = await GetPaymentProviderIdAsync(PaymentProviderType.ZaloPay);
        var paymentMethodId = await GetPaymentMethodIdAsync(PaymentMethodType.AtmCardDomestic);
        var order = await SeedOrderAsync(OrderPaymentStatus.Pending);

        var request = new CreatePaymentIntentRequest
        {
            PaymentProviderId = paymentProviderId,
            PaymentMethodId = paymentMethodId
        };

        var response = await _service.CreatePaymentIntentAsync(order.Id, request, CancellationToken.None);

        response.IsExistingIntent.Should().BeFalse();
        response.OrderId.Should().Be(order.Id);
        response.PaymentIntentStatus.Should().Be("Initiated");
        response.PaymentTransactionStatus.Should().Be("Pending");
        response.CheckoutUrl.Should().NotBeNullOrWhiteSpace();

        var savedOrder = await _dbContext.Orders
            .Include(x => x.PaymentOrder)
                .ThenInclude(x => x!.Status)
            .Include(x => x.PaymentOrder)
                .ThenInclude(x => x!.PaymentTransaction)
                    .ThenInclude(x => x!.Status)
            .SingleAsync(x => x.Id == order.Id);

        savedOrder.PaymentOrder.Should().NotBeNull();
        savedOrder.PaymentOrder!.Status.Status.Should().Be(PaymentIntentStatus.Initiated);
        savedOrder.PaymentOrder.PaymentTransaction.Should().NotBeNull();
        savedOrder.PaymentOrder.PaymentTransaction!.Status.Status.Should().Be(PaymentTransactionStatus.Pending);
    }

    [Fact]
    public async Task CreatePaymentIntentAsync_WhenExistingOpenIntent_ShouldReuseIntent()
    {
        var paymentProviderId = await GetPaymentProviderIdAsync(PaymentProviderType.ZaloPay);
        var paymentMethodId = await GetPaymentMethodIdAsync(PaymentMethodType.AtmCardDomestic);

        var order = await SeedOrderAsync(OrderPaymentStatus.Pending);
        var existing = await SeedPaymentIntentAggregateAsync(
            order,
            paymentProviderId,
            paymentMethodId,
            PaymentIntentStatus.Redirected,
            DateTime.UtcNow.AddMinutes(10));

        var request = new CreatePaymentIntentRequest
        {
            PaymentProviderId = paymentProviderId,
            PaymentMethodId = paymentMethodId
        };

        var existingTransactionCount = await _dbContext.PaymentTransactions.CountAsync();

        var response = await _service.CreatePaymentIntentAsync(order.Id, request, CancellationToken.None);

        response.IsExistingIntent.Should().BeTrue();
        response.PaymentOrderId.Should().Be(existing.PaymentOrder.Id);
        response.PaymentTransactionId.Should().Be(existing.PaymentTransaction.Id);

        var transactionCount = await _dbContext.PaymentTransactions.CountAsync();
        transactionCount.Should().Be(existingTransactionCount);
    }

    [Fact]
    public async Task CreatePaymentIntentAsync_WhenExistingIntentExpired_ShouldCreateNewTransactionForSamePaymentOrder()
    {
        var paymentProviderId = await GetPaymentProviderIdAsync(PaymentProviderType.ZaloPay);
        var paymentMethodId = await GetPaymentMethodIdAsync(PaymentMethodType.AtmCardDomestic);

        var order = await SeedOrderAsync(OrderPaymentStatus.Pending);
        var existing = await SeedPaymentIntentAggregateAsync(
            order,
            paymentProviderId,
            paymentMethodId,
            PaymentIntentStatus.Redirected,
            DateTime.UtcNow.AddMinutes(-10));

        var request = new CreatePaymentIntentRequest
        {
            PaymentProviderId = paymentProviderId,
            PaymentMethodId = paymentMethodId
        };

        var existingTransactionCount = await _dbContext.PaymentTransactions.CountAsync();

        var response = await _service.CreatePaymentIntentAsync(order.Id, request, CancellationToken.None);

        response.IsExistingIntent.Should().BeFalse();
        response.PaymentOrderId.Should().Be(existing.PaymentOrder.Id);
        response.PaymentTransactionId.Should().NotBe(existing.PaymentTransaction.Id);

        var updatedOrder = await _dbContext.Orders
            .Include(x => x.PaymentOrder)
            .SingleAsync(x => x.Id == order.Id);

        updatedOrder.PaymentOrder!.PaymentTransactionId.Should().Be(response.PaymentTransactionId);

        var transactionCount = await _dbContext.PaymentTransactions.CountAsync();
        transactionCount.Should().Be(existingTransactionCount + 1);
    }

    [Fact]
    public async Task CreatePaymentIntentAsync_WhenPaymentProviderIsInvalid_ShouldThrowInvalidOperationException()
    {
        var paymentMethodId = await GetPaymentMethodIdAsync(PaymentMethodType.AtmCardDomestic);
        var order = await SeedOrderAsync(OrderPaymentStatus.Pending);

        var request = new CreatePaymentIntentRequest
        {
            PaymentProviderId = Guid.NewGuid(),
            PaymentMethodId = paymentMethodId
        };

        var act = async () => await _service.CreatePaymentIntentAsync(order.Id, request, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Payment provider '*' was not found or is inactive.");
    }

    [Fact]
    public async Task CreatePaymentIntentAsync_WhenPaymentMethodIsInvalid_ShouldThrowInvalidOperationException()
    {
        var paymentProviderId = await GetPaymentProviderIdAsync(PaymentProviderType.ZaloPay);
        var order = await SeedOrderAsync(OrderPaymentStatus.Pending);

        var request = new CreatePaymentIntentRequest
        {
            PaymentProviderId = paymentProviderId,
            PaymentMethodId = Guid.NewGuid()
        };

        var act = async () => await _service.CreatePaymentIntentAsync(order.Id, request, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Payment method '*' was not found or is inactive.");
    }

    [Theory]
    [InlineData(OrderPaymentStatus.Paid)]
    [InlineData(OrderPaymentStatus.Cancelled)]
    public async Task CreatePaymentIntentAsync_WhenOrderStateDisallowsIntent_ShouldThrowInvalidOperationException(OrderPaymentStatus orderPaymentStatus)
    {
        var paymentProviderId = await GetPaymentProviderIdAsync(PaymentProviderType.ZaloPay);
        var paymentMethodId = await GetPaymentMethodIdAsync(PaymentMethodType.AtmCardDomestic);
        var order = await SeedOrderAsync(orderPaymentStatus);

        var request = new CreatePaymentIntentRequest
        {
            PaymentProviderId = paymentProviderId,
            PaymentMethodId = paymentMethodId
        };

        var act = async () => await _service.CreatePaymentIntentAsync(order.Id, request, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Order '{order.Id}' cannot create payment intent in status '{orderPaymentStatus}'.");
    }

    private async Task<Order> SeedOrderAsync(OrderPaymentStatus paymentStatus)
    {
        var statusId = await _dbContext.OrderPaymentStatusLookups
            .Where(x => x.Status == paymentStatus)
            .Select(x => x.Id)
            .SingleAsync();

        var currency = new CurrencyLookup
        {
            Id = Guid.NewGuid(),
            Code = $"VND-{Guid.NewGuid():N}"[..11],
            Name = "Vietnam Dong",
            Symbol = "VND",
            DecimalPlaces = 0,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var order = new Order
        {
            Id = Guid.NewGuid(),
            OrderCode = $"ORD-{Guid.NewGuid():N}"[..20],
            CurrencyId = currency.Id,
            TotalAmount = 250000m,
            PaymentStatusId = statusId,
            AmountPaid = paymentStatus == OrderPaymentStatus.Paid ? 250000m : 0m,
            PaidAtUtc = paymentStatus == OrderPaymentStatus.Paid ? DateTime.UtcNow : null,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _dbContext.CurrencyLookups.AddAsync(currency);
        await _dbContext.Orders.AddAsync(order);
        await _dbContext.SaveChangesAsync();

        return order;
    }

    private async Task<(PaymentOrder PaymentOrder, PaymentTransaction PaymentTransaction)> SeedPaymentIntentAggregateAsync(
        Order order,
        Guid paymentProviderId,
        Guid paymentMethodId,
        PaymentIntentStatus intentStatus,
        DateTime expiresAtUtc)
    {
        var intentStatusId = await _dbContext.PaymentIntentStatusLookups
            .Where(x => x.Status == intentStatus)
            .Select(x => x.Id)
            .SingleAsync();

        var txPendingStatusId = await _dbContext.PaymentTransactionStatusLookups
            .Where(x => x.Status == PaymentTransactionStatus.Pending)
            .Select(x => x.Id)
            .SingleAsync();

        var paymentTransaction = new PaymentTransaction
        {
            Id = Guid.NewGuid(),
            TransactionCode = $"TX-{Guid.NewGuid():N}"[..24],
            PaymentProviderId = paymentProviderId,
            PaymentMethodId = paymentMethodId,
            FromAccount = "customer-001",
            ToAccount = "merchant-001",
            Direction = PaymentTransactionDirection.InCome,
            Amount = order.TotalAmount,
            CurrencyId = order.CurrencyId,
            StatusId = txPendingStatusId,
            TransactionAtUtc = DateTime.UtcNow,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var paymentOrder = new PaymentOrder
        {
            Id = Guid.NewGuid(),
            OrderId = order.Id,
            PaymentProviderId = paymentProviderId,
            PaymentMethodId = paymentMethodId,
            StatusId = intentStatusId,
            IntentCode = $"PI-{Guid.NewGuid():N}"[..30],
            CheckoutUrl = "https://payments.local/checkout/seed",
            ExpiresAtUtc = expiresAtUtc,
            PaymentTransactionId = paymentTransaction.Id,
            OrderedAtUtc = DateTime.UtcNow,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        order.PaymentOrder = paymentOrder;
        order.UpdatedAt = DateTime.UtcNow;

        await _dbContext.PaymentTransactions.AddAsync(paymentTransaction);
        await _dbContext.PaymentOrders.AddAsync(paymentOrder);
        await _dbContext.SaveChangesAsync();

        return (paymentOrder, paymentTransaction);
    }

    private async Task<Guid> GetPaymentProviderIdAsync(PaymentProviderType providerType)
    {
        return await _dbContext.PaymentProviderLookups
            .Where(x => x.Provider == providerType)
            .Select(x => x.Id)
            .SingleAsync();
    }

    private async Task<Guid> GetPaymentMethodIdAsync(PaymentMethodType paymentMethodType)
    {
        return await _dbContext.PaymentMethodLookups
            .Where(x => x.Method == paymentMethodType)
            .Select(x => x.Id)
            .SingleAsync();
    }
}
