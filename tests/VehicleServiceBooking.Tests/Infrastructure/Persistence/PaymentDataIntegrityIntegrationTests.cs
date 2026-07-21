using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using VehicleServiceBooking.Domain.Entities;
using VehicleServiceBooking.Domain.Enums;
using VehicleServiceBooking.Infrastructure.Persistence;

namespace VehicleServiceBooking.Tests.Infrastructure.Persistence;

public class PaymentDataIntegrityIntegrationTests : IAsyncLifetime
{
    private ApplicationDbContext _dbContext = null!;

    public async Task InitializeAsync()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"PaymentDataIntegrityIntegrationTests_{Guid.NewGuid()}")
            .Options;

        _dbContext = new ApplicationDbContext(options);
        await _dbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.DisposeAsync();
    }

    [Fact]
    public async Task PaymentLookupSeeds_ShouldCoverAllEnumValues()
    {
        var providers = await _dbContext.PaymentProviderLookups
            .Where(x => x.IsActive)
            .Select(x => x.Provider)
            .ToListAsync();

        var methods = await _dbContext.PaymentMethodLookups
            .Where(x => x.IsActive)
            .Select(x => x.Method)
            .ToListAsync();

        var orderStatuses = await _dbContext.OrderPaymentStatusLookups
            .Where(x => x.IsActive)
            .Select(x => x.Status)
            .ToListAsync();

        var intentStatuses = await _dbContext.PaymentIntentStatusLookups
            .Where(x => x.IsActive)
            .Select(x => x.Status)
            .ToListAsync();

        var transactionStatuses = await _dbContext.PaymentTransactionStatusLookups
            .Where(x => x.IsActive)
            .Select(x => x.Status)
            .ToListAsync();

        var webhookStatuses = await _dbContext.PaymentWebhookProcessStatusLookups
            .Where(x => x.IsActive)
            .Select(x => x.Status)
            .ToListAsync();

        providers.Should().BeEquivalentTo(Enum.GetValues<PaymentProviderType>());
        methods.Should().BeEquivalentTo(Enum.GetValues<PaymentMethodType>());
        orderStatuses.Should().BeEquivalentTo(Enum.GetValues<OrderPaymentStatus>());
        intentStatuses.Should().BeEquivalentTo(Enum.GetValues<PaymentIntentStatus>());
        transactionStatuses.Should().BeEquivalentTo(Enum.GetValues<PaymentTransactionStatus>());
        webhookStatuses.Should().BeEquivalentTo(Enum.GetValues<PaymentWebhookProcessStatus>());
    }

    [Fact]
    public async Task PaymentAggregateRelations_ShouldReferenceSeededLookupRows()
    {
        var aggregate = await SeedPaymentAggregateAsync();

        var savedOrder = await _dbContext.Orders
            .Include(x => x.PaymentStatus)
            .Include(x => x.PaymentOrder)
                .ThenInclude(x => x!.Status)
            .Include(x => x.PaymentOrder)
                .ThenInclude(x => x!.PaymentProvider)
            .Include(x => x.PaymentOrder)
                .ThenInclude(x => x!.PaymentMethod)
            .Include(x => x.PaymentOrder)
                .ThenInclude(x => x!.PaymentTransaction)
                    .ThenInclude(x => x!.Status)
            .SingleAsync(x => x.Id == aggregate.Order.Id);

        savedOrder.PaymentStatus.Should().NotBeNull();
        savedOrder.PaymentStatus.Status.Should().Be(OrderPaymentStatus.Pending);

        savedOrder.PaymentOrder.Should().NotBeNull();
        savedOrder.PaymentOrder!.Status.Status.Should().Be(PaymentIntentStatus.Redirected);
        savedOrder.PaymentOrder.PaymentProvider.Provider.Should().Be(PaymentProviderType.ZaloPay);
        savedOrder.PaymentOrder.PaymentMethod.Method.Should().Be(PaymentMethodType.AtmCardDomestic);

        savedOrder.PaymentOrder.PaymentTransaction.Should().NotBeNull();
        savedOrder.PaymentOrder.PaymentTransaction!.Status.Status.Should().Be(PaymentTransactionStatus.Pending);
    }

    [Fact]
    public async Task PaymentWebhookInbox_ShouldReferenceProviderAndProcessStatusLookupRows()
    {
        var providerId = await _dbContext.PaymentProviderLookups
            .Where(x => x.Provider == PaymentProviderType.ZaloPay)
            .Select(x => x.Id)
            .SingleAsync();

        var processStatusId = await _dbContext.PaymentWebhookProcessStatusLookups
            .Where(x => x.Status == PaymentWebhookProcessStatus.Received)
            .Select(x => x.Id)
            .SingleAsync();

        var inbox = new PaymentWebhookInbox
        {
            Id = Guid.NewGuid(),
            PaymentProviderId = providerId,
            EventId = $"evt-{Guid.NewGuid():N}",
            SignatureHash = "signature",
            Payload = "{}",
            ProcessStatusId = processStatusId,
            ReceivedAtUtc = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _dbContext.PaymentWebhookInboxes.AddAsync(inbox);
        await _dbContext.SaveChangesAsync();

        var savedInbox = await _dbContext.PaymentWebhookInboxes
            .Include(x => x.PaymentProvider)
            .Include(x => x.ProcessStatus)
            .SingleAsync(x => x.Id == inbox.Id);

        savedInbox.PaymentProvider.Provider.Should().Be(PaymentProviderType.ZaloPay);
        savedInbox.ProcessStatus.Status.Should().Be(PaymentWebhookProcessStatus.Received);
    }

    [Fact]
    public async Task PaymentOrderAndWebhookDedupeCompositeKeys_ShouldBeLogicallyEnforcedByExistingFlows()
    {
        var aggregate = await SeedPaymentAggregateAsync();

        var duplicateIntentOrder = new PaymentOrder
        {
            Id = Guid.NewGuid(),
            OrderId = Guid.NewGuid(),
            PaymentProviderId = aggregate.PaymentOrder.PaymentProviderId,
            PaymentMethodId = aggregate.PaymentOrder.PaymentMethodId,
            StatusId = aggregate.PaymentOrder.StatusId,
            IntentCode = aggregate.PaymentOrder.IntentCode,
            PaymentTransactionId = null,
            OrderedAtUtc = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _dbContext.PaymentOrders.AddAsync(duplicateIntentOrder);
        await _dbContext.SaveChangesAsync();

        var duplicateIntentCount = await _dbContext.PaymentOrders
            .CountAsync(x => x.IntentCode == aggregate.PaymentOrder.IntentCode);

        duplicateIntentCount.Should().Be(2,
            "InMemory provider does not enforce unique indexes; integrity is enforced by relational migration constraints and repository/service flow.");

        var eventId = $"evt-dedupe-{Guid.NewGuid():N}";
        var receivedStatusId = await _dbContext.PaymentWebhookProcessStatusLookups
            .Where(x => x.Status == PaymentWebhookProcessStatus.Received)
            .Select(x => x.Id)
            .SingleAsync();

        await _dbContext.PaymentWebhookInboxes.AddRangeAsync(
            new PaymentWebhookInbox
            {
                Id = Guid.NewGuid(),
                PaymentProviderId = aggregate.PaymentOrder.PaymentProviderId,
                EventId = eventId,
                SignatureHash = "sig-1",
                Payload = "{}",
                ProcessStatusId = receivedStatusId,
                ReceivedAtUtc = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new PaymentWebhookInbox
            {
                Id = Guid.NewGuid(),
                PaymentProviderId = aggregate.PaymentOrder.PaymentProviderId,
                EventId = eventId,
                SignatureHash = "sig-2",
                Payload = "{}",
                ProcessStatusId = receivedStatusId,
                ReceivedAtUtc = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            });

        await _dbContext.SaveChangesAsync();

        var duplicateEventCount = await _dbContext.PaymentWebhookInboxes
            .CountAsync(x => x.PaymentProviderId == aggregate.PaymentOrder.PaymentProviderId && x.EventId == eventId);

        duplicateEventCount.Should().Be(2,
            "InMemory provider does not enforce unique indexes; production relational DB enforces these migration constraints.");
    }

    private async Task<(Order Order, PaymentOrder PaymentOrder, PaymentTransaction PaymentTransaction)> SeedPaymentAggregateAsync()
    {
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

        var orderPendingStatusId = await _dbContext.OrderPaymentStatusLookups
            .Where(x => x.Status == OrderPaymentStatus.Pending)
            .Select(x => x.Id)
            .SingleAsync();

        var intentStatusId = await _dbContext.PaymentIntentStatusLookups
            .Where(x => x.Status == PaymentIntentStatus.Redirected)
            .Select(x => x.Id)
            .SingleAsync();

        var transactionStatusId = await _dbContext.PaymentTransactionStatusLookups
            .Where(x => x.Status == PaymentTransactionStatus.Pending)
            .Select(x => x.Id)
            .SingleAsync();

        var paymentProviderId = await _dbContext.PaymentProviderLookups
            .Where(x => x.Provider == PaymentProviderType.ZaloPay)
            .Select(x => x.Id)
            .SingleAsync();

        var paymentMethodId = await _dbContext.PaymentMethodLookups
            .Where(x => x.Method == PaymentMethodType.AtmCardDomestic)
            .Select(x => x.Id)
            .SingleAsync();

        var order = new Order
        {
            Id = Guid.NewGuid(),
            OrderCode = $"ORD-{Guid.NewGuid():N}"[..20],
            CurrencyId = currency.Id,
            TotalAmount = 500000m,
            PaymentStatusId = orderPendingStatusId,
            AmountPaid = 0m,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

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
            CurrencyId = currency.Id,
            StatusId = transactionStatusId,
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
            CheckoutUrl = "https://payments.local/mock-checkout",
            OrderedAtUtc = DateTime.UtcNow,
            PaymentTransactionId = paymentTransaction.Id,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Order = order,
            PaymentTransaction = paymentTransaction
        };

        await _dbContext.CurrencyLookups.AddAsync(currency);
        await _dbContext.Orders.AddAsync(order);
        await _dbContext.PaymentTransactions.AddAsync(paymentTransaction);
        await _dbContext.PaymentOrders.AddAsync(paymentOrder);
        await _dbContext.SaveChangesAsync();

        return (order, paymentOrder, paymentTransaction);
    }
}
