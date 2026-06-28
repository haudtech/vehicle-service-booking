using FluentAssertions;
using Moq;
using VehicleServiceBooking.Application.Interfaces.Repositories;
using VehicleServiceBooking.Application.Models;
using VehicleServiceBooking.Application.Services;
using VehicleServiceBooking.Tests.Common;
using Microsoft.Extensions.Logging;
using Xunit;

namespace VehicleServiceBooking.Tests.Application.Services;

public class AvailabilityServiceTests
{
    private readonly Mock<ILogger<AvailabilityService>> _mockLogger;
    private readonly Mock<IAvailabilityRepository> _mockAvailabilityRepository;
    private readonly AvailabilityService _availabilityService;

    public AvailabilityServiceTests()
    {
        _mockLogger = new Mock<ILogger<AvailabilityService>>();
        _mockAvailabilityRepository = new Mock<IAvailabilityRepository>();

        _availabilityService = new AvailabilityService(
            _mockAvailabilityRepository.Object,
            _mockLogger.Object
        );
    }

    [Fact]
    public async Task GetAvailableSlotsAsync_WithValidInputs_ShouldReturnAvailableSlots()
    {
        var dealershipId = Guid.NewGuid();
        var serviceTypeId = Guid.NewGuid();
        var technicianId = Guid.NewGuid();
        var serviceBayId = Guid.NewGuid();
        var date = DateTime.UtcNow.AddDays(1).Date;

        SetupRepositoryAvailability(
            dealershipId,
            date,
            new List<AvailabilityProjection>
            {
                AvailabilityProjectionBuilder.CreateValid()
                    .WithSlotTimes(new TimeOnly(9, 0), new TimeOnly(10, 0))
                    .WithTechnicianId(technicianId)
                    .WithServiceBayId(serviceBayId)
                    .Build()
            });

        var result = await _availabilityService.GetAvailableSlotsAsync(
            dealershipId,
            serviceTypeId,
            date,
            CancellationToken.None);

        result.Should().NotBeEmpty();
        result.Should().ContainSingle();
        result[0].TechnicianId.Should().Be(technicianId);
        result[0].ServiceBayId.Should().Be(serviceBayId);
        result[0].DateTimeSlot.Start.Should().Be(date.AddHours(9));
        result[0].DateTimeSlot.End.Should().Be(date.AddHours(10));
    }

    [Fact]
    public async Task GetAvailableSlotsAsync_WithNoEligibleTechnicians_ShouldReturnEmptyList()
    {
        var dealershipId = Guid.NewGuid();
        var serviceTypeId = Guid.NewGuid();
        var date = DateTime.UtcNow.AddDays(1).Date;

        SetupRepositoryAvailability(dealershipId, date, new List<AvailabilityProjection>());

        var result = await _availabilityService.GetAvailableSlotsAsync(
            dealershipId,
            serviceTypeId,
            date,
            CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAvailableSlotsAsync_WithTechnicianBusyDuringSlot_ShouldNotReturnThatSlot()
    {
        var dealershipId = Guid.NewGuid();
        var serviceTypeId = Guid.NewGuid();
        var technicianId = Guid.NewGuid();
        var serviceBayId = Guid.NewGuid();
        var date = DateTime.UtcNow.AddDays(1).Date;

        SetupRepositoryAvailability(
            dealershipId,
            date,
            new List<AvailabilityProjection>
            {
                AvailabilityProjectionBuilder.CreateValid()
                    .WithSlotTimes(new TimeOnly(9, 30), new TimeOnly(10, 0))
                    .WithTechnicianId(technicianId)
                    .WithServiceBayId(serviceBayId)
                    .Build()
            });

        var result = await _availabilityService.GetAvailableSlotsAsync(
            dealershipId,
            serviceTypeId,
            date,
            CancellationToken.None);

        result.Should().NotBeEmpty();
        result.Should().AllSatisfy(slot => slot.DateTimeSlot.Start.Should().NotBe(date.AddHours(9)));
    }

    [Fact]
    public async Task GetAvailableSlotsAsync_WithNoServiceBays_ShouldReturnEmptyList()
    {
        var dealershipId = Guid.NewGuid();
        var serviceTypeId = Guid.NewGuid();
        var date = DateTime.UtcNow.AddDays(1).Date;

        SetupRepositoryAvailability(dealershipId, date, new List<AvailabilityProjection>());

        var result = await _availabilityService.GetAvailableSlotsAsync(
            dealershipId,
            serviceTypeId,
            date,
            CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAvailableSlotsAsync_WithMultipleTechnicians_ShouldReturnOptionsForAll()
    {
        var dealershipId = Guid.NewGuid();
        var serviceTypeId = Guid.NewGuid();
        var technician1Id = Guid.NewGuid();
        var technician2Id = Guid.NewGuid();
        var serviceBayId = Guid.NewGuid();
        var date = DateTime.UtcNow.AddDays(1).Date;

        SetupRepositoryAvailability(
            dealershipId,
            date,
            new List<AvailabilityProjection>
            {
                AvailabilityProjectionBuilder.CreateValid()
                    .WithSlotTimes(new TimeOnly(9, 0), new TimeOnly(10, 0))
                    .WithTechnicianId(technician1Id)
                    .WithServiceBayId(serviceBayId)
                    .Build(),
                AvailabilityProjectionBuilder.CreateValid()
                    .WithSlotTimes(new TimeOnly(10, 0), new TimeOnly(11, 0))
                    .WithTechnicianId(technician2Id)
                    .WithServiceBayId(serviceBayId)
                    .Build()
            });

        var result = await _availabilityService.GetAvailableSlotsAsync(
            dealershipId,
            serviceTypeId,
            date,
            CancellationToken.None);

        var uniqueTechnicians = result.Select(r => r.TechnicianId).Distinct();
        uniqueTechnicians.Should().Contain(new[] { technician1Id, technician2Id });
    }

    [Fact]
    public async Task GetAvailableSlotsAsync_WithLongerServiceDuration_ShouldCalculateCorrectSlotCount()
    {
        var dealershipId = Guid.NewGuid();
        var serviceTypeId = Guid.NewGuid();
        var technicianId = Guid.NewGuid();
        var serviceBayId = Guid.NewGuid();
        var date = DateTime.UtcNow.AddDays(1).Date;

        SetupRepositoryAvailability(
            dealershipId,
            date,
            new List<AvailabilityProjection>
            {
                AvailabilityProjectionBuilder.CreateValid()
                    .WithSlotTimes(new TimeOnly(9, 0), new TimeOnly(10, 30))
                    .WithTechnicianId(technicianId)
                    .WithServiceBayId(serviceBayId)
                    .Build()
            });

        var result = await _availabilityService.GetAvailableSlotsAsync(
            dealershipId,
            serviceTypeId,
            date,
            CancellationToken.None);

        result.Should().AllSatisfy(slot =>
        {
            var duration = slot.DateTimeSlot.End - slot.DateTimeSlot.Start;
            duration.Should().Be(TimeSpan.FromMinutes(90));
        });
    }

    [Fact]
    public async Task GetAvailableSlotsAsync_OutsideTechnicianSchedule_ShouldNotReturnSlot()
    {
        var dealershipId = Guid.NewGuid();
        var serviceTypeId = Guid.NewGuid();
        var technicianId = Guid.NewGuid();
        var serviceBayId = Guid.NewGuid();
        var date = DateTime.UtcNow.AddDays(1).Date;

        SetupRepositoryAvailability(
            dealershipId,
            date,
            new List<AvailabilityProjection>
            {
                AvailabilityProjectionBuilder.CreateValid()
                    .WithSlotTimes(new TimeOnly(10, 0), new TimeOnly(10, 30))
                    .WithTechnicianId(technicianId)
                    .WithServiceBayId(serviceBayId)
                    .Build()
            });

        var result = await _availabilityService.GetAvailableSlotsAsync(
            dealershipId,
            serviceTypeId,
            date,
            CancellationToken.None);

        result.Should().AllSatisfy(slot =>
        {
            slot.DateTimeSlot.Start.TimeOfDay.Should().BeGreaterThanOrEqualTo(new TimeSpan(10, 0, 0));
            slot.DateTimeSlot.End.TimeOfDay.Should().BeLessThanOrEqualTo(new TimeSpan(14, 0, 0));
        });
    }

    private void SetupRepositoryAvailability(
        Guid dealershipId,
        DateTime date,
        IEnumerable<AvailabilityProjection> projections)
    {
        _mockAvailabilityRepository
            .Setup(r => r.GetServiceTypeAvailabilityAsync(
                dealershipId,
                It.IsAny<Guid[]>(),
                DateOnly.FromDateTime(date),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(projections);
    }
}
