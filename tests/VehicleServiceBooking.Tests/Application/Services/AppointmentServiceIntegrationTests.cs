using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using VehicleServiceBooking.Application.DTOs;
using VehicleServiceBooking.Application.Interfaces;
using VehicleServiceBooking.Application.Exceptions;
using VehicleServiceBooking.Application.Interfaces.Repositories;
using VehicleServiceBooking.Application.Interfaces.Services;
using VehicleServiceBooking.Application.Models;
using VehicleServiceBooking.Application.Services;
using VehicleServiceBooking.Domain.Entities;
using VehicleServiceBooking.Domain.Enums;
using VehicleServiceBooking.Infrastructure.Persistence;
using VehicleServiceBooking.Infrastructure.Repositories;
using VehicleServiceBooking.Tests.Common;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;

namespace VehicleServiceBooking.Tests.Application.Services;

/// <summary>
/// Integration tests for AppointmentService using in-memory database with builder pattern
/// These tests use AppointmentIntegrationScenarioBuilder for clean, reusable test data setup
/// </summary>
public class AppointmentServiceIntegrationTests : IAsyncLifetime
{
    private ApplicationDbContext _dbContext = null!;
    private AppointmentService _appointmentService = null!;
    private IAppointmentRepository _appointmentRepository = null!;
    private Mock<IAvailabilityService> _mockAvailabilityService = null!;
    private Mock<ILogger<AppointmentService>> _mockLogger = null!;

    public async Task InitializeAsync()
    {
        // Create in-memory database with unique name
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"test_{Guid.NewGuid()}")
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _appointmentRepository = new AppointmentRepository(_dbContext);
        var timeSlotRepository = new TimeSlotRepository(_dbContext);
        var appointmentStatusLookupRepository = new AppointmentStatusLookupRepository(_dbContext);
        var serviceStatusLookupRepository = new ServiceStatusLookupRepository(_dbContext);
        _mockAvailabilityService = new Mock<IAvailabilityService>();
        _mockLogger = new Mock<ILogger<AppointmentService>>();

        _mockAvailabilityService
            .Setup(s => s.GetAvailableSlotsAsync(
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid dealershipId, Guid serviceTypeId, DateTime date, CancellationToken cancellationToken) =>
                new List<AvailabilityOption>
                {
                    AvailabilityOptionBuilder.CreateValid()
                        .WithTimeSlot(
                            AppTimeSlotBuilder.CreateValid()
                                .WithTimes(new TimeOnly(8, 0), new TimeOnly(8, 30))
                                .Build())
                        .WithTechnicianId(Guid.Empty)
                        .WithServiceBayId(Guid.Empty)
                        .Build()
                });

        _appointmentService = new AppointmentService(
            _appointmentRepository,
            timeSlotRepository,
            appointmentStatusLookupRepository,
            serviceStatusLookupRepository,
            _mockAvailabilityService.Object,
            _mockLogger.Object
        );

        // Ensure database is created
        await _dbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        // Clean up resources
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.DisposeAsync();
    }

    private void SetupAvailabilityServiceForRequest(Guid technicianId, Guid serviceBayId, DateTime date)
    {
        _mockAvailabilityService
            .Setup(s => s.GetAvailableSlotsAsync(
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AvailabilityOption>
            {
                AvailabilityOptionBuilder.CreateValid()
                    .WithTimeSlot(
                        AppTimeSlotBuilder.CreateValid()
                            .WithTimes(new TimeOnly(8, 0), new TimeOnly(8, 30))
                            .Build())
                    .WithTechnicianId(technicianId)
                    .WithServiceBayId(serviceBayId)
                    .Build()
            });
    }

    private void SeedTechnicianSkills(IEnumerable<Guid> technicianIds, Guid serviceTypeId)
    {
        _dbContext.TechnicianSkills.AddRange(
            technicianIds.Select(id => TechnicianSkillBuilder.ValidSkill()
                .WithTechnicianId(id)
                .WithServiceTypeId(serviceTypeId)
                .Build()));
    }

    #region CreateAppointmentAsync Integration Tests

    [Fact]
    public async Task CreateAppointmentAsync_WithAllValidEntities_ShouldCreateAppointment()
    {
        // Arrange - Use builder to create complete setup
        var (dealership, customer, vehicle, serviceType, technicians, serviceBays, _) =
            AppointmentIntegrationScenarioBuilder.CompleteSetup()
                .Build();

        _dbContext.Dealerships.Add(dealership);
        _dbContext.Customers.Add(customer);
        _dbContext.Vehicles.Add(vehicle);
        _dbContext.ServiceTypes.Add(serviceType);
        _dbContext.Technicians.AddRange(technicians);
        _dbContext.ServiceBays.AddRange(serviceBays);
        SeedTechnicianSkills(technicians.Select(t => t.Id), serviceType.Id);

        await _dbContext.SaveChangesAsync();

        var appointmentDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var startTimeSlotId = Guid.Parse("00000000-0000-0000-0000-000000000001");  // Slot 1: 08:00-08:30
        var endTimeSlotId = Guid.Parse("00000000-0000-0000-0000-000000000002");    // Slot 2: 08:30-09:00

        var request = new CreateAppointmentRequestBuilder()
            .WithDealershipId(dealership.Id)
            .WithCustomerId(customer.Id)
            .WithVehicleId(vehicle.Id)
            .WithAppointmentDate(appointmentDate)
            .WithServiceTypeId(serviceType.Id)
            .WithTechnicianId(technicians[0].Id)
            .WithServiceBayId(serviceBays[0].Id)
            .WithTimeSlots(startTimeSlotId, endTimeSlotId)
            .Build();

        SetupAvailabilityServiceForRequest(request.TechnicianId, request.ServiceBayId, request.AppointmentDate.ToDateTime(TimeOnly.MinValue));

        // Act
        var result = await _appointmentService.CreateAppointmentAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.AppointmentId.Should().NotBeEmpty();
        result.SlotStart.Should().Be(appointmentDate.ToDateTime(new TimeOnly(8, 0)));   // Slot 1 starts at 08:00
        result.SlotEnd.Should().Be(appointmentDate.ToDateTime(new TimeOnly(9, 0)));     // Slot 2 ends at 09:00

        // Verify appointment was saved to database
        var savedAppointment = await _dbContext.Appointments
            .Include(a => a.Status)
            .FirstOrDefaultAsync(a => a.Id == result.AppointmentId);
        savedAppointment.Should().NotBeNull();
        savedAppointment!.DealershipId.Should().Be(dealership.Id);
        savedAppointment.CustomerId.Should().Be(customer.Id);
        savedAppointment.Status!.Status.Should().Be(AppointmentStatus.Booked);
    }

    [Fact]
    public async Task CreateAppointmentAsync_WithNonExistentDealership_ShouldCreateAppointment()
    {
        // Arrange - Create setup and use non-existent dealership
        var (dealership, customer, vehicle, serviceType, technicians, serviceBays, _) =
            AppointmentIntegrationScenarioBuilder.CompleteSetup()
                .Build();

        _dbContext.Customers.Add(customer);
        _dbContext.Vehicles.Add(vehicle);
        _dbContext.ServiceTypes.Add(serviceType);
        _dbContext.Technicians.AddRange(technicians);
        _dbContext.ServiceBays.AddRange(serviceBays);
        SeedTechnicianSkills(technicians.Select(t => t.Id), serviceType.Id);

        await _dbContext.SaveChangesAsync();

        var request = new CreateAppointmentRequestBuilder()
            .WithDealershipId(Guid.NewGuid()) // Non-existent dealership
            .WithCustomerId(customer.Id)
            .WithVehicleId(vehicle.Id)
            .WithAppointmentDate(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)))
            .WithServiceTypeId(serviceType.Id)
            .WithTechnicianId(technicians[0].Id)
            .WithServiceBayId(serviceBays[0].Id)
            .WithTimeSlots(
                Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Guid.Parse("00000000-0000-0000-0000-000000000002"))
            .Build();

        SetupAvailabilityServiceForRequest(request.TechnicianId, request.ServiceBayId, request.AppointmentDate.ToDateTime(TimeOnly.MinValue));

        // Act - Service allows non-existent Dealership (it's validated by foreign key at DB level)
        var result = await _appointmentService.CreateAppointmentAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.AppointmentId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateAppointmentAsync_WithConflictingTimeSlot_ShouldThrowException()
    {
        // Arrange - Use builder to create complete setup with existing appointments
        var (dealership, customer, vehicle, serviceType, technicians, serviceBays, existingAppointments) =
            AppointmentIntegrationScenarioBuilder.WithExistingAppointments()
                .Build();

        _dbContext.Dealerships.Add(dealership);
        _dbContext.Customers.Add(customer);
        _dbContext.Vehicles.Add(vehicle);
        _dbContext.ServiceTypes.Add(serviceType);
        _dbContext.Technicians.AddRange(technicians);
        _dbContext.ServiceBays.AddRange(serviceBays);

        // Conflict validation is vehicle-based in AppointmentService.
        // Ensure seeded existing appointments target the same vehicle/date.
        foreach (var appointment in existingAppointments)
        {
            appointment.VehicleId = vehicle.Id;
            appointment.AppointmentDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        }

        _dbContext.Appointments.AddRange(existingAppointments);

        SeedTechnicianSkills(technicians.Select(t => t.Id), serviceType.Id);
        await _dbContext.SaveChangesAsync();

        // Request for overlapping slot using TimeSlot IDs
        var conflictingRequest = new CreateAppointmentRequestBuilder()
            .WithDealershipId(dealership.Id)
            .WithCustomerId(customer.Id)
            .WithVehicleId(vehicle.Id)
            .WithAppointmentDate(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)))
            .WithServiceTypeId(serviceType.Id)
            .WithTechnicianId(technicians[0].Id)
            .WithServiceBayId(existingAppointments[0].Services.First().ServiceBayId!.Value)
            .WithTimeSlots(
                Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Guid.Parse("00000000-0000-0000-0000-000000000002"))
            .Build();

        SetupAvailabilityServiceForRequest(conflictingRequest.TechnicianId, conflictingRequest.ServiceBayId, conflictingRequest.AppointmentDate.ToDateTime(TimeOnly.MinValue));

        // Act & Assert
        await Assert.ThrowsAsync<BookingConflictException>(
            () => _appointmentService.CreateAppointmentAsync(conflictingRequest)
        );
    }

    [Fact]
    public async Task CreateAppointmentAsync_WithNonConflictingAppointments_ShouldSucceed()
    {
        // Arrange - Use builder with existing appointments
        var (dealership, customer, vehicle, serviceType, technicians, serviceBays, existingAppointments) =
            AppointmentIntegrationScenarioBuilder.WithExistingAppointments()
                .Build();

        _dbContext.Dealerships.Add(dealership);
        _dbContext.Customers.Add(customer);
        _dbContext.Vehicles.Add(vehicle);
        _dbContext.ServiceTypes.Add(serviceType);
        _dbContext.Technicians.AddRange(technicians);
        _dbContext.ServiceBays.AddRange(serviceBays);
        _dbContext.Appointments.AddRange(existingAppointments);
        SeedTechnicianSkills(technicians.Select(t => t.Id), serviceType.Id);

        await _dbContext.SaveChangesAsync();

        // Request for non-overlapping slot (different time slots)
        var request = new CreateAppointmentRequestBuilder()
            .WithDealershipId(dealership.Id)
            .WithCustomerId(customer.Id)
            .WithVehicleId(vehicle.Id)
            .WithAppointmentDate(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)))
            .WithServiceTypeId(serviceType.Id)
            .WithTechnicianId(technicians[0].Id)
            .WithServiceBayId(existingAppointments[0].Services.First().ServiceBayId!.Value)
            .WithTimeSlots(
                Guid.Parse("00000000-0000-0000-0000-000000000003"),
                Guid.Parse("00000000-0000-0000-0000-000000000004"))
            .Build();

        SetupAvailabilityServiceForRequest(request.TechnicianId, request.ServiceBayId, request.AppointmentDate.ToDateTime(TimeOnly.MinValue));

        // Act
        var result = await _appointmentService.CreateAppointmentAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.AppointmentId.Should().NotBeEmpty();

        // Verify both appointments exist
        var allAppointments = await _dbContext.Appointments.ToListAsync();
        allAppointments.Should().HaveCountGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task CreateAppointmentAsync_WithSameTechnicianDifferentBay_ShouldSucceed()
    {
        // Arrange - Use builder with multiple service bays and existing appointments
        var (dealership, customer, vehicle, serviceType, technicians, serviceBays, existingAppointments) =
            AppointmentIntegrationScenarioBuilder.MultipleServiceBays()
                .IncludeExistingAppointments(true)
                .Build();

        _dbContext.Dealerships.Add(dealership);
        _dbContext.Customers.Add(customer);
        _dbContext.Vehicles.Add(vehicle);
        _dbContext.ServiceTypes.Add(serviceType);
        _dbContext.Technicians.AddRange(technicians);
        _dbContext.ServiceBays.AddRange(serviceBays);
        _dbContext.Appointments.AddRange(existingAppointments);
        SeedTechnicianSkills(technicians.Select(t => t.Id), serviceType.Id);

        await _dbContext.SaveChangesAsync();

        var appointmentDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

        // Create second appointment with same technician in different bay (same time) - should succeed
        var request = new CreateAppointmentRequestBuilder()
            .WithDealershipId(dealership.Id)
            .WithCustomerId(customer.Id)
            .WithVehicleId(vehicle.Id)
            .WithAppointmentDate(appointmentDate)
            .WithServiceTypeId(serviceType.Id)
            .WithTechnicianId(technicians[0].Id)
            .WithServiceBayId(serviceBays[1].Id)
            .WithTimeSlots(
                Guid.Parse("00000000-0000-0000-0000-000000000005"),
                Guid.Parse("00000000-0000-0000-0000-000000000006"))
            .Build();

        SetupAvailabilityServiceForRequest(request.TechnicianId, request.ServiceBayId, request.AppointmentDate.ToDateTime(TimeOnly.MinValue));

        // Act
        var result = await _appointmentService.CreateAppointmentAsync(request);

        // Assert - Service only checks service bay conflicts, not technician conflicts
        result.Should().NotBeNull();
        result.AppointmentId.Should().NotBeEmpty();

        var createdAppointment = await _dbContext.Appointments.FindAsync(result.AppointmentId);
        createdAppointment.Should().NotBeNull();
        createdAppointment!.Services.Should().HaveCount(1);
        createdAppointment.Services.First().TechnicianId.Should().Be(technicians[0].Id);
        createdAppointment.Services.First().ServiceBayId.Should().Be(serviceBays[1].Id);
    }

    [Fact]
    public async Task CreateAppointmentAsync_WithConflictingServiceBay_ShouldThrowException()
    {
        // Arrange - Use builder with multiple technicians and existing appointments
        var (dealership, customer, vehicle, serviceType, technicians, serviceBays, existingAppointments) =
            AppointmentIntegrationScenarioBuilder.MultipleTechnicians()
                .IncludeExistingAppointments(true)
                .Build();

        _dbContext.Dealerships.Add(dealership);
        _dbContext.Customers.Add(customer);
        _dbContext.Vehicles.Add(vehicle);
        _dbContext.ServiceTypes.Add(serviceType);
        _dbContext.Technicians.AddRange(technicians);
        _dbContext.ServiceBays.AddRange(serviceBays);
        _dbContext.Appointments.AddRange(existingAppointments);

        await _dbContext.SaveChangesAsync();

        var firstAppointment = existingAppointments[0];
        var startTime = firstAppointment.Services.Min(s => s.EstimatedStartTime) ?? DateTime.UtcNow;
        var endTime = firstAppointment.Services.Max(s => s.EstimatedEndTime) ?? DateTime.UtcNow.AddHours(1);

        // Try to create second appointment in same service bay with overlapping time
        var request = new CreateAppointmentRequestBuilder()
            .WithDealershipId(dealership.Id)
            .WithCustomerId(customer.Id)
            .WithVehicleId(vehicle.Id)
            .WithAppointmentDate(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)))
            .WithServiceTypeId(serviceType.Id)
            .WithTechnicianId(technicians[1].Id)
            .WithServiceBayId(firstAppointment.Services.First().ServiceBayId!.Value)
            .WithTimeSlots(
                Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Guid.Parse("00000000-0000-0000-0000-000000000002"))
            .Build();

        SetupAvailabilityServiceForRequest(request.TechnicianId, request.ServiceBayId, request.AppointmentDate.ToDateTime(TimeOnly.MinValue));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _appointmentService.CreateAppointmentAsync(request)
        );
    }

    #endregion

    #region GetAppointmentByIdAsync Integration Tests

    [Fact]
    public async Task GetAppointmentByIdAsync_WithValidId_ShouldReturnAppointment()
    {
        // Arrange - Use builder with existing appointments
        var (dealership, customer, vehicle, serviceType, technicians, serviceBays, existingAppointments) =
            AppointmentIntegrationScenarioBuilder.CompleteSetup()
                .IncludeExistingAppointments(true)
                .Build();

        _dbContext.Dealerships.Add(dealership);
        _dbContext.Customers.Add(customer);
        _dbContext.Vehicles.Add(vehicle);
        _dbContext.ServiceTypes.Add(serviceType);
        _dbContext.Technicians.AddRange(technicians);
        _dbContext.ServiceBays.AddRange(serviceBays);
        _dbContext.Appointments.AddRange(existingAppointments);

        await _dbContext.SaveChangesAsync();

        var targetAppointment = existingAppointments[0];

        // Act
        var result = await _appointmentService.GetAppointmentByIdAsync(targetAppointment.Id);

        // Assert
        result.Should().NotBeNull();
        result!.AppointmentId.Should().Be(targetAppointment.Id);
        result.SlotStart.Should().Be(targetAppointment.AppointmentDate.ToDateTime(new TimeOnly(8, 0)));
        result.SlotEnd.Should().Be(targetAppointment.AppointmentDate.ToDateTime(new TimeOnly(9, 0)));
    }

    [Fact]
    public async Task GetAppointmentByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Act
        var result = await _appointmentService.GetAppointmentByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAppointmentByIdAsync_WithMultipleAppointments_ShouldReturnCorrectOne()
    {
        // Arrange - Use builder with existing appointments
        var (dealership, customer, vehicle, serviceType, technicians, serviceBays, existingAppointments) =
            AppointmentIntegrationScenarioBuilder.WithExistingAppointments()
                .Build();

        _dbContext.Dealerships.Add(dealership);
        _dbContext.Customers.Add(customer);
        _dbContext.Vehicles.Add(vehicle);
        _dbContext.ServiceTypes.Add(serviceType);
        _dbContext.Technicians.AddRange(technicians);
        _dbContext.ServiceBays.AddRange(serviceBays);
        _dbContext.Appointments.AddRange(existingAppointments);

        await _dbContext.SaveChangesAsync();

        var targetAppointment = existingAppointments[0];

        // Act
        var result = await _appointmentService.GetAppointmentByIdAsync(targetAppointment.Id);

        // Assert
        result.Should().NotBeNull();
        result!.AppointmentId.Should().Be(targetAppointment.Id);
        result.SlotStart.Should().Be(targetAppointment.AppointmentDate.ToDateTime(new TimeOnly(8, 0)));
        result.SlotEnd.Should().Be(targetAppointment.AppointmentDate.ToDateTime(new TimeOnly(9, 0)));
    }

    [Fact]
    public async Task GetAppointmentByIdAsync_AfterAppointmentCreation_ShouldReturnCreatedAppointment()
    {
        // Arrange - Use builder to create complete setup
        var (dealership, customer, vehicle, serviceType, technicians, serviceBays, _) =
            AppointmentIntegrationScenarioBuilder.CompleteSetup()
                .Build();

        _dbContext.Dealerships.Add(dealership);
        _dbContext.Customers.Add(customer);
        _dbContext.Vehicles.Add(vehicle);
        _dbContext.ServiceTypes.Add(serviceType);
        _dbContext.Technicians.AddRange(technicians);
        _dbContext.ServiceBays.AddRange(serviceBays);
        SeedTechnicianSkills(technicians.Select(t => t.Id), serviceType.Id);

        await _dbContext.SaveChangesAsync();

        var appointmentDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

        var request = new CreateAppointmentRequestBuilder()
            .WithDealershipId(dealership.Id)
            .WithCustomerId(customer.Id)
            .WithVehicleId(vehicle.Id)
            .WithAppointmentDate(appointmentDate)
            .WithServiceTypeId(serviceType.Id)
            .WithTechnicianId(technicians[0].Id)
            .WithServiceBayId(serviceBays[0].Id)
            .WithTimeSlots(
                Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Guid.Parse("00000000-0000-0000-0000-000000000002"))
            .Build();

        SetupAvailabilityServiceForRequest(request.TechnicianId, request.ServiceBayId, request.AppointmentDate.ToDateTime(TimeOnly.MinValue));

        // Act
        var createdResponse = await _appointmentService.CreateAppointmentAsync(request);
        var retrievedResponse = await _appointmentService.GetAppointmentByIdAsync(createdResponse.AppointmentId);

        // Assert
        retrievedResponse.Should().NotBeNull();
        retrievedResponse!.AppointmentId.Should().Be(createdResponse.AppointmentId);
        retrievedResponse.SlotStart.Should().Be(createdResponse.SlotStart);
        retrievedResponse.SlotEnd.Should().Be(createdResponse.SlotEnd);
    }

    #endregion

    #region Database Transaction Tests

    [Fact]
    public async Task CreateAppointmentAsync_ShouldPersistChangesToDatabase()
    {
        // Arrange - Use builder to create complete setup
        var (dealership, customer, vehicle, serviceType, technicians, serviceBays, _) =
            AppointmentIntegrationScenarioBuilder.CompleteSetup()
                .Build();

        _dbContext.Dealerships.Add(dealership);
        _dbContext.Customers.Add(customer);
        _dbContext.Vehicles.Add(vehicle);
        _dbContext.ServiceTypes.Add(serviceType);
        _dbContext.Technicians.AddRange(technicians);
        _dbContext.ServiceBays.AddRange(serviceBays);
        SeedTechnicianSkills(technicians.Select(t => t.Id), serviceType.Id);

        await _dbContext.SaveChangesAsync();

        var appointmentDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

        var request = new CreateAppointmentRequestBuilder()
            .WithDealershipId(dealership.Id)
            .WithCustomerId(customer.Id)
            .WithVehicleId(vehicle.Id)
            .WithAppointmentDate(appointmentDate)
            .WithServiceTypeId(serviceType.Id)
            .WithTechnicianId(technicians[0].Id)
            .WithServiceBayId(serviceBays[0].Id)
            .WithTimeSlots(
                Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Guid.Parse("00000000-0000-0000-0000-000000000002"))
            .Build();

        SetupAvailabilityServiceForRequest(request.TechnicianId, request.ServiceBayId, request.AppointmentDate.ToDateTime(TimeOnly.MinValue));

        // Act
        var response = await _appointmentService.CreateAppointmentAsync(request);

        // Assert - Query database directly to verify persistence
        var dbAppointment = await _dbContext.Appointments.FirstOrDefaultAsync(
            a => a.Id == response.AppointmentId
        );

        dbAppointment.Should().NotBeNull();
        dbAppointment!.DealershipId.Should().Be(dealership.Id);
        dbAppointment.CustomerId.Should().Be(customer.Id);
    }

    [Fact]
    public async Task MultipleAppointmentCreations_ShouldAllBeRetrievable()
    {
        // Arrange - Use builder to create complete setup
        var (dealership, customer, vehicle, serviceType, technicians, serviceBays, _) =
            AppointmentIntegrationScenarioBuilder.CompleteSetup()
                .Build();

        _dbContext.Dealerships.Add(dealership);
        _dbContext.Customers.Add(customer);
        _dbContext.Vehicles.Add(vehicle);
        _dbContext.ServiceTypes.Add(serviceType);
        _dbContext.Technicians.AddRange(technicians);
        _dbContext.ServiceBays.AddRange(serviceBays);
        SeedTechnicianSkills(technicians.Select(t => t.Id), serviceType.Id);

        await _dbContext.SaveChangesAsync();

        var appointmentIds = new List<Guid>();

        // Act - Create 5 appointments
        for (int i = 0; i < 5; i++)
        {
            var appointmentDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
            var request = new CreateAppointmentRequestBuilder()
                .WithDealershipId(dealership.Id)
                .WithCustomerId(customer.Id)
                .WithVehicleId(vehicle.Id)
                .WithAppointmentDate(appointmentDate)
                .WithServiceTypeId(serviceType.Id)
                .WithTechnicianId(technicians[0].Id)
                .WithServiceBayId(serviceBays[0].Id)
                .WithTimeSlots(
                    Guid.Parse($"00000000-0000-0000-0000-{(i+1):000000000000}"),
                    Guid.Parse($"00000000-0000-0000-0000-{(i+2):000000000000}"))
                .Build();

            SetupAvailabilityServiceForRequest(request.TechnicianId, request.ServiceBayId, request.AppointmentDate.ToDateTime(TimeOnly.MinValue));

            var response = await _appointmentService.CreateAppointmentAsync(request);
            appointmentIds.Add(response.AppointmentId);
        }

        // Assert - All should be retrievable
        for (int i = 0; i < appointmentIds.Count; i++)
        {
            var result = await _appointmentService.GetAppointmentByIdAsync(appointmentIds[i]);
            result.Should().NotBeNull();
        }

        // Verify count
        var allAppointments = await _dbContext.Appointments.ToListAsync();
        allAppointments.Should().HaveCountGreaterThanOrEqualTo(5);
    }

    #endregion
}
