using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using VehicleServiceBooking.Application.DTOs;
using VehicleServiceBooking.Application.Interfaces;
using VehicleServiceBooking.Application.Interfaces.Repositories;
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
        _mockAvailabilityService = new Mock<IAvailabilityService>();
        _mockLogger = new Mock<ILogger<AppointmentService>>();

        _appointmentService = new AppointmentService(
            _dbContext,
            _appointmentRepository,
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

        await _dbContext.SaveChangesAsync();

        var startTime = DateTime.UtcNow.AddHours(1);
        var endTime = startTime.AddHours(1);

        var request = new CreateAppointmentRequest
        {
            DealershipId = dealership.Id,
            CustomerId = customer.Id,
            VehicleId = vehicle.Id,
            ServiceTypeId = serviceType.Id,
            TechnicianId = technicians[0].Id,
            ServiceBayId = serviceBays[0].Id,
            SlotStart = startTime,
            SlotEnd = endTime
        };

        // Act
        var result = await _appointmentService.CreateAppointmentAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.AppointmentId.Should().NotBeEmpty();
        result.SlotStart.Should().Be(startTime);
        result.SlotEnd.Should().Be(endTime);

        // Verify appointment was saved to database
        var savedAppointment = await _dbContext.Appointments.FindAsync(result.AppointmentId);
        savedAppointment.Should().NotBeNull();
        savedAppointment!.DealershipId.Should().Be(dealership.Id);
        savedAppointment.CustomerId.Should().Be(customer.Id);
        savedAppointment.Status.Should().Be(AppointmentStatus.Booked);
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

        await _dbContext.SaveChangesAsync();

        var request = new CreateAppointmentRequest
        {
            DealershipId = Guid.NewGuid(), // Non-existent dealership
            CustomerId = customer.Id,
            VehicleId = vehicle.Id,
            ServiceTypeId = serviceType.Id,
            TechnicianId = technicians[0].Id,
            ServiceBayId = serviceBays[0].Id,
            SlotStart = DateTime.UtcNow.AddHours(1),
            SlotEnd = DateTime.UtcNow.AddHours(2)
        };

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
        _dbContext.Appointments.AddRange(existingAppointments);

        await _dbContext.SaveChangesAsync();

        var startTime = existingAppointments[0].StartTime;
        var endTime = existingAppointments[0].EndTime;

        // Request for overlapping slot
        var conflictingRequest = new CreateAppointmentRequest
        {
            DealershipId = dealership.Id,
            CustomerId = customer.Id,
            VehicleId = vehicle.Id,
            ServiceTypeId = serviceType.Id,
            TechnicianId = technicians[0].Id,
            ServiceBayId = existingAppointments[0].ServiceBayId,
            SlotStart = startTime,
            SlotEnd = endTime
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
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

        await _dbContext.SaveChangesAsync();

        // Request for non-overlapping slot (after existing appointment)
        var existingEnd = existingAppointments[0].EndTime;
        var newStart = existingEnd.AddHours(2);
        var newEnd = newStart.AddHours(1);

        var request = new CreateAppointmentRequest
        {
            DealershipId = dealership.Id,
            CustomerId = customer.Id,
            VehicleId = vehicle.Id,
            ServiceTypeId = serviceType.Id,
            TechnicianId = technicians[0].Id,
            ServiceBayId = existingAppointments[0].ServiceBayId,
            SlotStart = newStart,
            SlotEnd = newEnd
        };

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

        await _dbContext.SaveChangesAsync();

        var startTime = DateTime.UtcNow.AddHours(2);
        var endTime = startTime.AddHours(1);

        // Create second appointment with same technician in different bay (same time) - should succeed
        var request = new CreateAppointmentRequest
        {
            DealershipId = dealership.Id,
            CustomerId = customer.Id,
            VehicleId = vehicle.Id,
            ServiceTypeId = serviceType.Id,
            TechnicianId = technicians[0].Id,
            ServiceBayId = serviceBays[1].Id,
            SlotStart = startTime.AddMinutes(15),
            SlotEnd = endTime.AddMinutes(15)
        };

        // Act
        var result = await _appointmentService.CreateAppointmentAsync(request);

        // Assert - Service only checks service bay conflicts, not technician conflicts
        result.Should().NotBeNull();
        result.AppointmentId.Should().NotBeEmpty();

        var createdAppointment = await _dbContext.Appointments.FindAsync(result.AppointmentId);
        createdAppointment.Should().NotBeNull();
        createdAppointment!.TechnicianId.Should().Be(technicians[0].Id);
        createdAppointment.ServiceBayId.Should().Be(serviceBays[1].Id);
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
        var startTime = firstAppointment.StartTime;
        var endTime = firstAppointment.EndTime;

        // Try to create second appointment in same service bay with overlapping time
        var request = new CreateAppointmentRequest
        {
            DealershipId = dealership.Id,
            CustomerId = customer.Id,
            VehicleId = vehicle.Id,
            ServiceTypeId = serviceType.Id,
            TechnicianId = technicians[1].Id, // Different technician
            ServiceBayId = firstAppointment.ServiceBayId, // Same bay!
            SlotStart = startTime.AddMinutes(15),
            SlotEnd = endTime.AddMinutes(15)
        };

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
        result.AppointmentId.Should().Be(targetAppointment.Id);
        result.SlotStart.Should().Be(targetAppointment.StartTime);
        result.SlotEnd.Should().Be(targetAppointment.EndTime);
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
        result.AppointmentId.Should().Be(targetAppointment.Id);
        result.SlotStart.Should().Be(targetAppointment.StartTime);
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

        await _dbContext.SaveChangesAsync();

        var startTime = DateTime.UtcNow.AddHours(1);
        var endTime = startTime.AddHours(1);

        var request = new CreateAppointmentRequest
        {
            DealershipId = dealership.Id,
            CustomerId = customer.Id,
            VehicleId = vehicle.Id,
            ServiceTypeId = serviceType.Id,
            TechnicianId = technicians[0].Id,
            ServiceBayId = serviceBays[0].Id,
            SlotStart = startTime,
            SlotEnd = endTime
        };

        // Act
        var createdResponse = await _appointmentService.CreateAppointmentAsync(request);
        var retrievedResponse = await _appointmentService.GetAppointmentByIdAsync(createdResponse.AppointmentId);

        // Assert
        retrievedResponse.Should().NotBeNull();
        retrievedResponse.AppointmentId.Should().Be(createdResponse.AppointmentId);
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

        await _dbContext.SaveChangesAsync();

        var startTime = DateTime.UtcNow.AddHours(1);

        var request = new CreateAppointmentRequest
        {
            DealershipId = dealership.Id,
            CustomerId = customer.Id,
            VehicleId = vehicle.Id,
            ServiceTypeId = serviceType.Id,
            TechnicianId = technicians[0].Id,
            ServiceBayId = serviceBays[0].Id,
            SlotStart = startTime,
            SlotEnd = startTime.AddHours(1)
        };

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

        await _dbContext.SaveChangesAsync();

        var appointmentIds = new List<Guid>();

        // Act - Create 5 appointments
        for (int i = 0; i < 5; i++)
        {
            var startTime = DateTime.UtcNow.AddHours(i + 1);
            var request = new CreateAppointmentRequest
            {
                DealershipId = dealership.Id,
                CustomerId = customer.Id,
                VehicleId = vehicle.Id,
                ServiceTypeId = serviceType.Id,
                TechnicianId = technicians[0].Id,
                ServiceBayId = serviceBays[0].Id,
                SlotStart = startTime,
                SlotEnd = startTime.AddHours(1)
            };

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
