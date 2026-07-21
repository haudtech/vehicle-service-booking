using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using VehicleServiceBooking.Application.Configuration;
using VehicleServiceBooking.Application.DTOs;
using VehicleServiceBooking.Application.Exceptions;
using VehicleServiceBooking.Application.Helpers;
using VehicleServiceBooking.Application.Services;
using VehicleServiceBooking.Domain.Entities;
using VehicleServiceBooking.Domain.Enums;
using VehicleServiceBooking.Infrastructure.Persistence;
using VehicleServiceBooking.Infrastructure.Repositories;

namespace VehicleServiceBooking.Tests.Application.Services;

public class PaymentWebhookServiceIntegrationTests : IAsyncLifetime
{
    private const string SignatureSecret = "unit-test-shared-secret";

    private ApplicationDbContext _dbContext = null!;
    private PaymentWebhookService _service = null!;

    public async Task InitializeAsync()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"PaymentWebhookServiceIntegrationTests_{Guid.NewGuid()}")
            .Options;

        _dbContext = new ApplicationDbContext(options);
        await _dbContext.Database.EnsureCreatedAsync();

        var repository = new PaymentWebhookRepository(_dbContext);
        _service = new PaymentWebhookService(
            repository,
            Options.Create(new PaymentWebhookSecurityOptions
            {
                Enabled = true,
                RequireSignature = true,
                SharedSecret = SignatureSecret
            }),
            Mock.Of<ILogger<PaymentWebhookService>>());
    }

    public async Task DisposeAsync()
    {
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.DisposeAsync();
    }

    [Fact]
    public async Task ProcessAsync_WithValidSignature_ShouldProcessAndUpdatePaidState()
    {
        var intentCode = "INT-PAID-001";
        var providerType = PaymentProviderType.ZaloPay;
        var occurredAt = DateTime.UtcNow;
        await SeedPaymentAggregateAsync(intentCode, providerType);

        var request = BuildSignedRequest(
            providerType,
            eventId: $"evt-{Guid.NewGuid():N}",
            intentCode,
            transactionStatus: PaymentTransactionStatus.Completed,
            occurredAtUtc: occurredAt);

        var response = await _service.ProcessAsync(providerType, request, CancellationToken.None);

        response.IsDuplicate.Should().BeFalse();
        response.ProcessStatus.Should().Be("Processed");

        var paymentOrder = await _dbContext.PaymentOrders
            .Include(x => x.Status)
            .Include(x => x.Order)
                .ThenInclude(x => x.PaymentStatus)
            .Include(x => x.PaymentTransaction)
                .ThenInclude(x => x!.Status)
            .SingleAsync(x => x.IntentCode == intentCode);

        paymentOrder.Status.Status.Should().Be(PaymentIntentStatus.Paid);
        paymentOrder.LastProviderEventId.Should().Be(request.EventId);
        paymentOrder.LastProviderEventAtUtc.Should().Be(occurredAt);

        paymentOrder.Order.PaymentStatus.Status.Should().Be(OrderPaymentStatus.Paid);
        paymentOrder.Order.AmountPaid.Should().Be(paymentOrder.Order.TotalAmount);
        paymentOrder.Order.PaidAtUtc.Should().Be(occurredAt);

        paymentOrder.PaymentTransaction.Should().NotBeNull();
        paymentOrder.PaymentTransaction!.Status.Status.Should().Be(PaymentTransactionStatus.Completed);
        paymentOrder.PaymentTransaction.CompletedAtUtc.Should().Be(occurredAt);

        var webhookInbox = await _dbContext.PaymentWebhookInboxes
            .Include(x => x.ProcessStatus)
            .SingleAsync(x => x.EventId == request.EventId);

        webhookInbox.ProcessStatus.Status.Should().Be(PaymentWebhookProcessStatus.Processed);
        webhookInbox.ProcessedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public async Task ProcessAsync_WithInvalidSignature_ShouldThrowPaymentWebhookSignatureValidationException()
    {
        var request = new ProcessPaymentWebhookRequest
        {
            EventId = $"evt-{Guid.NewGuid():N}",
            IntentCode = "INT-INVALID-SIG",
            TransactionStatus = PaymentTransactionStatus.Completed.ToString(),
            ProviderTransactionId = "mock-provider-tx",
            OccurredAtUtc = DateTime.UtcNow,
            SignatureHash = "BAD_SIGNATURE"
        };

        var act = async () => await _service.ProcessAsync(PaymentProviderType.ZaloPay, request, CancellationToken.None);

        await act.Should().ThrowAsync<PaymentWebhookSignatureValidationException>();
    }

    [Fact]
    public async Task ProcessAsync_WhenEventIdAlreadyExists_ShouldReturnDuplicateAndKeepSingleInboxRecord()
    {
        var providerType = PaymentProviderType.ZaloPay;
        var eventId = $"evt-dup-{Guid.NewGuid():N}";
        var intentCode = "INT-DEDUPE-001";

        await SeedPaymentAggregateAsync(intentCode, providerType);

        var request = BuildSignedRequest(
            providerType,
            eventId,
            intentCode,
            PaymentTransactionStatus.Completed,
            DateTime.UtcNow);

        var firstResponse = await _service.ProcessAsync(providerType, request, CancellationToken.None);
        var secondResponse = await _service.ProcessAsync(providerType, request, CancellationToken.None);

        firstResponse.IsDuplicate.Should().BeFalse();
        secondResponse.IsDuplicate.Should().BeTrue();

        var providerId = await _dbContext.PaymentProviderLookups
            .Where(x => x.Provider == providerType)
            .Select(x => x.Id)
            .SingleAsync();

        var inboxCount = await _dbContext.PaymentWebhookInboxes
            .CountAsync(x => x.PaymentProviderId == providerId && x.EventId == eventId);

        inboxCount.Should().Be(1);
    }

    private ProcessPaymentWebhookRequest BuildSignedRequest(
        PaymentProviderType providerType,
        string eventId,
        string intentCode,
        PaymentTransactionStatus transactionStatus,
        DateTime occurredAtUtc)
    {
        var request = new ProcessPaymentWebhookRequest
        {
            EventId = eventId,
            IntentCode = intentCode,
            TransactionStatus = transactionStatus.ToString(),
            ProviderTransactionId = $"provider-tx-{Guid.NewGuid():N}",
            OccurredAtUtc = occurredAtUtc,
            Payload = "{\"source\":\"integration-test\"}"
        };

        var canonicalMessage = PaymentWebhookSignatureHelper.BuildCanonicalMessage(providerType, request);
        request.SignatureHash = PaymentWebhookSignatureHelper.ComputeSignature(SignatureSecret, canonicalMessage);

        return request;
    }

    private async Task SeedPaymentAggregateAsync(string intentCode, PaymentProviderType providerType)
    {
        var currency = new CurrencyLookup
        {
            Id = Guid.NewGuid(),
            Code = "VND",
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

        var intentRedirectedStatusId = await _dbContext.PaymentIntentStatusLookups
            .Where(x => x.Status == PaymentIntentStatus.Redirected)
            .Select(x => x.Id)
            .SingleAsync();

        var txPendingStatusId = await _dbContext.PaymentTransactionStatusLookups
            .Where(x => x.Status == PaymentTransactionStatus.Pending)
            .Select(x => x.Id)
            .SingleAsync();

        var providerId = await _dbContext.PaymentProviderLookups
            .Where(x => x.Provider == providerType)
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
            AmountPaid = 0m,
            PaymentStatusId = orderPendingStatusId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var paymentTransaction = new PaymentTransaction
        {
            Id = Guid.NewGuid(),
            TransactionCode = $"TX-{Guid.NewGuid():N}"[..24],
            PaymentProviderId = providerId,
            PaymentMethodId = paymentMethodId,
            FromAccount = "customer-001",
            ToAccount = "dealer-001",
            Direction = PaymentTransactionDirection.InCome,
            Amount = order.TotalAmount,
            CurrencyId = currency.Id,
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
            PaymentProviderId = providerId,
            PaymentMethodId = paymentMethodId,
            StatusId = intentRedirectedStatusId,
            IntentCode = intentCode,
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
    }
}
