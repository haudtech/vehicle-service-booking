using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using VehicleServiceBooking.Application.Configuration;
using VehicleServiceBooking.Application.DTOs;
using VehicleServiceBooking.Application.Interfaces.Repositories;
using VehicleServiceBooking.Application.Services;
using VehicleServiceBooking.Domain.Entities;
using VehicleServiceBooking.Domain.Enums;

namespace VehicleServiceBooking.Tests.Application.Services;

public class PaymentWebhookServiceTransactionTests
{
    [Fact]
    public async Task ProcessAsync_WhenUniqueInboxCollisionOccurs_ShouldReturnDuplicateResponse()
    {
        var providerType = PaymentProviderType.ZaloPay;
        var providerId = Guid.NewGuid();
        var eventId = $"evt-race-{Guid.NewGuid():N}";

        var transaction = new Mock<IDbContextTransaction>();
        transaction
            .Setup(x => x.RollbackAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        transaction
            .Setup(x => x.DisposeAsync())
            .Returns(ValueTask.CompletedTask);

        var repository = new Mock<IPaymentWebhookRepository>();
        repository
            .Setup(x => x.GetActivePaymentProviderIdAsync(providerType, It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerId);

        repository
            .Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(transaction.Object);

        repository
            .SetupSequence(x => x.GetInboxByProviderAndEventIdAsync(providerId, eventId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PaymentWebhookInbox?)null)
            .ReturnsAsync(new PaymentWebhookInbox
            {
                Id = Guid.NewGuid(),
                EventId = eventId,
                ProcessStatus = new PaymentWebhookProcessStatusLookup
                {
                    Id = Guid.NewGuid(),
                    Name = "Processed",
                    Status = PaymentWebhookProcessStatus.Processed,
                    Description = "Processed",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

        repository
            .Setup(x => x.GetWebhookProcessStatusAsync(PaymentWebhookProcessStatus.Received, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaymentWebhookProcessStatusLookup
            {
                Id = Guid.NewGuid(),
                Name = "Received",
                Status = PaymentWebhookProcessStatus.Received,
                Description = "Received",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

        repository
            .Setup(x => x.AddWebhookInboxAsync(It.IsAny<PaymentWebhookInbox>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DbUpdateException("duplicate inbox", new Exception("IX_PaymentWebhookInbox_Provider_Event_Unique")));

        var service = new PaymentWebhookService(
            repository.Object,
            Options.Create(new PaymentWebhookSecurityOptions
            {
                Enabled = false,
                RequireSignature = false,
                SharedSecret = "unused"
            }),
            Mock.Of<ILogger<PaymentWebhookService>>());

        var request = new ProcessPaymentWebhookRequest
        {
            EventId = eventId,
            IntentCode = "INT-RACE-001",
            TransactionStatus = PaymentTransactionStatus.Completed.ToString(),
            ProviderTransactionId = "provider-tx-race",
            Payload = "{}",
            SignatureHash = "ignored-when-security-disabled",
            OccurredAtUtc = DateTime.UtcNow
        };

        var response = await service.ProcessAsync(providerType, request, CancellationToken.None);

        response.IsDuplicate.Should().BeTrue();
        response.EventId.Should().Be(eventId);
        response.Message.Should().ContainEquivalentOf("already processed");

        repository.Verify(x => x.ClearChangeTracker(), Times.Once);
        transaction.Verify(x => x.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
