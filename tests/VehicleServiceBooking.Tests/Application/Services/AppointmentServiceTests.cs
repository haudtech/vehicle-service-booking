using FluentAssertions;
using Moq;
using VehicleServiceBooking.Application.DTOs;
using VehicleServiceBooking.Application.Interfaces.Repositories;
using VehicleServiceBooking.Application.Interfaces.Services;
using VehicleServiceBooking.Application.Models;
using VehicleServiceBooking.Application.Services;
using VehicleServiceBooking.Domain.Entities;
using VehicleServiceBooking.Tests.Common;
using Microsoft.Extensions.Logging;
using Xunit;

namespace VehicleServiceBooking.Tests.Application.Services;

public class AppointmentServiceTests
{
    private readonly Mock<IAppointmentRepository> _mockAppointmentRepository;
    private readonly Mock<IAvailabilityService> _mockAvailabilityService;
    private readonly Mock<ILogger<AppointmentService>> _mockLogger;
    private readonly AppointmentService _appointmentService;

    public AppointmentServiceTests()
    {
        _mockAppointmentRepository = new Mock<IAppointmentRepository>();
        _mockAvailabilityService = new Mock<IAvailabilityService>();
        _mockLogger = new Mock<ILogger<AppointmentService>>();

        _appointmentService = new AppointmentService(
            _mockAppointmentRepository.Object,
            _mockAvailabilityService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task CreateAppointmentAsync_WithValidRequest_ShouldCreateAppointment()
    {
        var request = new CreateAppointmentRequestBuilder().Build();
        var createdAppointmentId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;

        SetupAvailability(request, true);
        _mockAppointmentRepository
            .Setup(r => r.TechnicianHasSkillAsync(request.TechnicianId, request.ServiceTypeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockAppointmentRepository
            .Setup(r => r.GetByVehicleIdAsync(request.VehicleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Appointment>());
        _mockAppointmentRepository
            .Setup(r => r.CreateAppointmentWithServicesAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Appointment a, CancellationToken _) =>
            {
                a.Id = createdAppointmentId;
                a.CreatedAt = createdAt;
                return a;
            });
        _mockAppointmentRepository
            .Setup(r => r.GetByIdAsync(createdAppointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BuildAppointmentFromRequest(request, createdAppointmentId, createdAt));
        _mockAppointmentRepository
            .Setup(r => r.GetTimeSlotsBySequenceRangeAsync(0, int.MaxValue, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BuildTimeSlotsForRequest(request));

        var result = await _appointmentService.CreateAppointmentAsync(request);

        result.AppointmentId.Should().Be(createdAppointmentId);
        result.SlotStart.Should().Be(request.AppointmentDate.ToDateTime(new TimeOnly(8, 0)));
        result.SlotEnd.Should().Be(request.AppointmentDate.ToDateTime(new TimeOnly(9, 0)));
        result.CreatedAt.Should().Be(createdAt);
    }

    [Fact]
    public async Task CreateAppointmentAsync_WithNoAvailability_ShouldThrowException()
    {
        var request = new CreateAppointmentRequestBuilder().Build();

        _mockAvailabilityService
            .Setup(s => s.GetAvailableSlotsAsync(
                request.DealershipId,
                request.ServiceTypeId,
                request.AppointmentDate.ToDateTime(TimeOnly.MinValue),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AvailabilityOption>());

        await Assert.ThrowsAsync<InvalidOperationException>(() => _appointmentService.CreateAppointmentAsync(request));
    }

    [Fact]
    public async Task CreateAppointmentAsync_WithUnavailableTechnicianBayCombination_ShouldThrowException()
    {
        var request = new CreateAppointmentRequestBuilder().Build();

        _mockAvailabilityService
            .Setup(s => s.GetAvailableSlotsAsync(
                request.DealershipId,
                request.ServiceTypeId,
                request.AppointmentDate.ToDateTime(TimeOnly.MinValue),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AvailabilityOption>
            {
                AvailabilityOptionBuilder.CreateValid()
                    .WithTimeSlot(
                        AppTimeSlotBuilder.CreateValid()
                            .WithTimes(new TimeOnly(8, 0), new TimeOnly(9, 0))
                            .Build())
                    .WithTechnicianId(Guid.NewGuid())
                    .WithServiceBayId(Guid.NewGuid())
                    .Build()
            });

        await Assert.ThrowsAsync<InvalidOperationException>(() => _appointmentService.CreateAppointmentAsync(request));
    }

    [Fact]
    public async Task GetAppointmentByIdAsync_WithValidId_ShouldReturnAppointment()
    {
        var appointmentId = Guid.NewGuid();
        var appointmentDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var request = new CreateAppointmentRequestBuilder().WithAppointmentDate(appointmentDate).Build();

        _mockAppointmentRepository
            .Setup(r => r.GetByIdAsync(appointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BuildAppointmentFromRequest(request, appointmentId, DateTime.UtcNow));
        _mockAppointmentRepository
            .Setup(r => r.GetTimeSlotsBySequenceRangeAsync(0, int.MaxValue, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BuildTimeSlotsForRequest(request));

        var result = await _appointmentService.GetAppointmentByIdAsync(appointmentId);

        result.Should().NotBeNull();
        result!.AppointmentId.Should().Be(appointmentId);
    }

    [Fact]
    public async Task GetAppointmentByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        var invalidId = Guid.NewGuid();

        _mockAppointmentRepository
            .Setup(r => r.GetByIdAsync(invalidId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Appointment?)null);

        var result = await _appointmentService.GetAppointmentByIdAsync(invalidId);

        result.Should().BeNull();
    }

    private void SetupAvailability(CreateAppointmentRequest request, bool hasMatchingSlot)
    {
        var options = new List<AvailabilityOption>();

        if (hasMatchingSlot)
        {
            options.Add(
                AvailabilityOptionBuilder.CreateValid()
                    .WithTimeSlot(
                        AppTimeSlotBuilder.CreateValid()
                            .WithTimes(new TimeOnly(8, 0), new TimeOnly(9, 0))
                            .Build())
                    .WithTechnicianId(request.TechnicianId)
                    .WithServiceBayId(request.ServiceBayId)
                    .Build());
        }

        _mockAvailabilityService
            .Setup(s => s.GetAvailableSlotsAsync(
                request.DealershipId,
                request.ServiceTypeId,
                request.AppointmentDate.ToDateTime(TimeOnly.MinValue),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(options);
    }

    private static Appointment BuildAppointmentFromRequest(
        CreateAppointmentRequest request,
        Guid appointmentId,
        DateTime createdAt)
    {
        var service = ServiceEntityBuilder.CreateValid()
            .WithAppointmentId(appointmentId)
            .WithServiceTypeId(request.ServiceTypeId)
            .WithTechnicianId(request.TechnicianId)
            .WithServiceBayId(request.ServiceBayId)
            .WithDealershipId(request.DealershipId)
            .WithSequenceOrder(1)
            .WithEstimatedSlotIds(request.EstimatedStartTimeSlotId, request.EstimatedEndTimeSlotId)
            .Build();

        return AppointmentEntityBuilder.CreateValid()
            .WithId(appointmentId)
            .WithDealershipId(request.DealershipId)
            .WithCustomerId(request.CustomerId)
            .WithVehicleId(request.VehicleId)
            .WithAppointmentDate(request.AppointmentDate)
            .WithStatusId(Guid.Parse("00000000-0000-0000-0000-000000000001"))
            .WithTimestamps(createdAt, createdAt)
            .WithServices(new[] { service })
            .Build();
    }

    private static IEnumerable<VehicleServiceBooking.Domain.Entities.TimeSlot> BuildTimeSlotsForRequest(CreateAppointmentRequest request)
    {
        return new List<VehicleServiceBooking.Domain.Entities.TimeSlot>
        {
            DomainTimeSlotBuilder.CreateValid()
                .WithId(request.EstimatedStartTimeSlotId)
                .WithSequenceOrder(1)
                .WithSlotTimes(new TimeOnly(8, 0), new TimeOnly(8, 30))
                .Build(),
            DomainTimeSlotBuilder.CreateValid()
                .WithId(request.EstimatedEndTimeSlotId)
                .WithSequenceOrder(2)
                .WithSlotTimes(new TimeOnly(8, 30), new TimeOnly(9, 0))
                .Build()
        };
    }
}
