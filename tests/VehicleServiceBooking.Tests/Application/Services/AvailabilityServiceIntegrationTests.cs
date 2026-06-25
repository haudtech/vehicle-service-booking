using FluentAssertions;
using VehicleServiceBooking.Application.Configuration.Interfaces;
using VehicleServiceBooking.Application.Interfaces;
using VehicleServiceBooking.Application.Interfaces.Repositories;
using VehicleServiceBooking.Application.Interfaces.Services;
using VehicleServiceBooking.Application.Services;
using VehicleServiceBooking.Domain.Entities;
using VehicleServiceBooking.Infrastructure.Persistence;
using VehicleServiceBooking.Infrastructure.Repositories;
using VehicleServiceBooking.Tests.Common;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace VehicleServiceBooking.Tests.Application.Services;

/// <summary>
/// Integration tests for AvailabilityService using in-memory database with builder pattern
/// These tests use AvailabilityIntegrationScenarioBuilder for clean, reusable test data setup
/// </summary>
public class AvailabilityServiceIntegrationTests : IAsyncLifetime
{
    private ApplicationDbContext _dbContext = null!;
    private IAvailabilityService _availabilityService = null!;
    private ISchedulingConfiguration _schedulingConfiguration = null!;

    public async Task InitializeAsync()
    {
        // Create an in-memory database for testing
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"AvailabilityServiceIntegrationTests_{Guid.NewGuid()}")
            .Options;

        _dbContext = new ApplicationDbContext(options);
        await _dbContext.Database.EnsureCreatedAsync();

        _schedulingConfiguration = new SchedulingConfiguration();
        _availabilityService = new AvailabilityService(
            _schedulingConfiguration,
            _dbContext
        );
    }

    public async Task DisposeAsync()
    {
        await _dbContext.Database.EnsureDeletedAsync();
        _dbContext.Dispose();
    }

    #region GetAvailableSlotsAsync Integration Tests

    [Fact]
    public async Task GetAvailableSlotsAsync_WithCompleteSetup_ShouldReturnAvailableSlots()
    {
        // Arrange
        var date = DateTime.UtcNow.AddDays(1).Date.AddHours(9);
        var (dealership, serviceType, technicians, technicianSkills, technicianSchedules, serviceBays, _) =
            AvailabilityIntegrationScenarioBuilder.CompleteSetup()
                .Build();

        _dbContext.Dealerships.Add(dealership);
        _dbContext.ServiceTypes.Add(serviceType);
        _dbContext.Technicians.AddRange(technicians);
        _dbContext.TechnicianSkills.AddRange(technicianSkills);
        _dbContext.TechnicianSchedules.AddRange(technicianSchedules);
        _dbContext.ServiceBays.AddRange(serviceBays);

        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _availabilityService.GetAvailableSlotsAsync(
            dealership.Id,
            serviceType.Id,
            date,
            CancellationToken.None
        );

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        result.Should().AllSatisfy(slot =>
        {
            slot.TimeSlot.Should().NotBeNull();
            slot.TechnicianId.Should().Be(technicians[0].Id);
            slot.ServiceBayId.Should().Be(serviceBays[0].Id);
        });
    }

    [Fact]
    public async Task GetAvailableSlotsAsync_WithExistingAppointment_ShouldNotReturnConflictingSlot()
    {
        // Arrange
        var date = DateTime.UtcNow.AddDays(1).Date.AddHours(9);
        var (dealership, serviceType, technicians, technicianSkills, technicianSchedules, serviceBays, existingAppointments) =
            AvailabilityIntegrationScenarioBuilder.WithExistingAppointmentConflict()
                .Build();

        _dbContext.Dealerships.Add(dealership);
        _dbContext.ServiceTypes.Add(serviceType);
        _dbContext.Technicians.AddRange(technicians);
        _dbContext.TechnicianSkills.AddRange(technicianSkills);
        _dbContext.TechnicianSchedules.AddRange(technicianSchedules);
        _dbContext.ServiceBays.AddRange(serviceBays);
        _dbContext.Appointments.AddRange(existingAppointments);

        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _availabilityService.GetAvailableSlotsAsync(
            dealership.Id,
            serviceType.Id,
            date,
            CancellationToken.None
        );

        // Assert
        result.Should().NotBeNull();
        // Should not have a slot at 9:00-9:30 (conflicting with existing appointment)
        result.Should().NotContain(slot =>
            slot.TimeSlot.Start == date.Date.AddHours(9) &&
            slot.TimeSlot.End == date.Date.AddHours(9).AddMinutes(30)
        );
    }

    [Fact]
    public async Task GetAvailableSlotsAsync_WithMultipleTechnicians_ShouldReturnOptionsForAll()
    {
        // Arrange
        var date = DateTime.UtcNow.AddDays(1).Date.AddHours(9);
        var (dealership, serviceType, technicians, technicianSkills, technicianSchedules, serviceBays, _) =
            AvailabilityIntegrationScenarioBuilder.MultipleTechnicians()
                .Build();

        _dbContext.Dealerships.Add(dealership);
        _dbContext.ServiceTypes.Add(serviceType);
        _dbContext.Technicians.AddRange(technicians);
        _dbContext.TechnicianSkills.AddRange(technicianSkills);
        _dbContext.TechnicianSchedules.AddRange(technicianSchedules);
        _dbContext.ServiceBays.AddRange(serviceBays);

        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _availabilityService.GetAvailableSlotsAsync(
            dealership.Id,
            serviceType.Id,
            date,
            CancellationToken.None
        );

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        
        var uniqueTechnicians = result.Select(r => r.TechnicianId).Distinct();
        uniqueTechnicians.Should().Contain(technicians.Select(t => t.Id));
    }

    [Fact]
    public async Task GetAvailableSlotsAsync_WithMultipleServiceBays_ShouldProvideMoreOptions()
    {
        // Arrange
        var date = DateTime.UtcNow.AddDays(1).Date.AddHours(9);
        var (dealership, serviceType, technicians, technicianSkills, technicianSchedules, serviceBays, _) =
            AvailabilityIntegrationScenarioBuilder.MultipleServiceBays()
                .Build();

        _dbContext.Dealerships.Add(dealership);
        _dbContext.ServiceTypes.Add(serviceType);
        _dbContext.Technicians.AddRange(technicians);
        _dbContext.TechnicianSkills.AddRange(technicianSkills);
        _dbContext.TechnicianSchedules.AddRange(technicianSchedules);
        _dbContext.ServiceBays.AddRange(serviceBays);

        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _availabilityService.GetAvailableSlotsAsync(
            dealership.Id,
            serviceType.Id,
            date,
            CancellationToken.None
        );

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        
        var uniqueBays = result.Select(r => r.ServiceBayId).Distinct();
        uniqueBays.Should().Contain(serviceBays.Select(b => b.Id));
    }

    [Fact]
    public async Task GetAvailableSlotsAsync_WithLongerServiceDuration_ShouldReturnLongerSlots()
    {
        // Arrange
        var date = DateTime.UtcNow.AddDays(1).Date.AddHours(9);
        var (dealership, serviceType, technicians, technicianSkills, technicianSchedules, serviceBays, _) =
            AvailabilityIntegrationScenarioBuilder.LongerServiceDuration()
                .Build();

        _dbContext.Dealerships.Add(dealership);
        _dbContext.ServiceTypes.Add(serviceType);
        _dbContext.Technicians.AddRange(technicians);
        _dbContext.TechnicianSkills.AddRange(technicianSkills);
        _dbContext.TechnicianSchedules.AddRange(technicianSchedules);
        _dbContext.ServiceBays.AddRange(serviceBays);

        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _availabilityService.GetAvailableSlotsAsync(
            dealership.Id,
            serviceType.Id,
            date,
            CancellationToken.None
        );

        // Assert
        result.Should().NotBeNull();
        result.Should().AllSatisfy(slot =>
        {
            var duration = slot.TimeSlot.End - slot.TimeSlot.Start;
            duration.Should().Be(TimeSpan.FromMinutes(90));
        });
    }

    [Fact]
    public async Task GetAvailableSlotsAsync_WithRestrictedTechnicianSchedule_ShouldRespectWorkingHours()
    {
        // Arrange
        var date = DateTime.UtcNow.AddDays(1).Date.AddHours(9);
        var (dealership, serviceType, technicians, technicianSkills, technicianSchedules, serviceBays, _) =
            AvailabilityIntegrationScenarioBuilder.RestrictedTechnicianSchedule()
                .Build();

        _dbContext.Dealerships.Add(dealership);
        _dbContext.ServiceTypes.Add(serviceType);
        _dbContext.Technicians.AddRange(technicians);
        _dbContext.TechnicianSkills.AddRange(technicianSkills);
        _dbContext.TechnicianSchedules.AddRange(technicianSchedules);
        _dbContext.ServiceBays.AddRange(serviceBays);

        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _availabilityService.GetAvailableSlotsAsync(
            dealership.Id,
            serviceType.Id,
            date,
            CancellationToken.None
        );

        // Assert
        result.Should().NotBeNull();
        // All slots should respect technician's working hours (10 AM - 2 PM)
        result.Should().AllSatisfy(slot =>
        {
            slot.TimeSlot.Start.TimeOfDay.Should().BeGreaterThanOrEqualTo(new TimeSpan(10, 0, 0));
            slot.TimeSlot.End.TimeOfDay.Should().BeLessThanOrEqualTo(new TimeSpan(14, 0, 0));
        });
    }

    [Fact]
    public async Task GetAvailableSlotsAsync_WithNonExistentServiceType_ShouldThrowException()
    {
        // Arrange
        var (dealership, _, _, _, _, _, _) =
            AvailabilityIntegrationScenarioBuilder.CompleteSetup()
                .Build();

        _dbContext.Dealerships.Add(dealership);
        await _dbContext.SaveChangesAsync();

        var nonExistentServiceTypeId = Guid.NewGuid();
        var date = DateTime.UtcNow.AddDays(1).Date.AddHours(9);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await _availabilityService.GetAvailableSlotsAsync(
                dealership.Id,
                nonExistentServiceTypeId,
                date,
                CancellationToken.None
            );
        });
    }

    [Fact]
    public async Task GetAvailableSlotsAsync_WithNoTechniciansForServiceType_ShouldReturnEmptyList()
    {
        // Arrange - Create only dealership and service bays without any technicians for a specific service type
        var (dealership, serviceType, _, _, _, serviceBays, _) =
            AvailabilityIntegrationScenarioBuilder.CompleteSetup()
                .Build();

        // Create a different service type that has no technicians
        var noTechServiceType = new ServiceType
        {
            Id = Guid.NewGuid(),
            Name = "Tire Rotation",
            DurationMinutes = 30
        };

        _dbContext.Dealerships.Add(dealership);
        _dbContext.ServiceTypes.Add(noTechServiceType);
        _dbContext.ServiceBays.AddRange(serviceBays);

        await _dbContext.SaveChangesAsync();

        var date = DateTime.UtcNow.AddDays(1).Date.AddHours(9);

        // Act
        var result = await _availabilityService.GetAvailableSlotsAsync(
            dealership.Id,
            noTechServiceType.Id,
            date,
            CancellationToken.None
        );

        // Assert
        result.Should().BeEmpty();
    }

    #endregion
}

/// <summary>
/// Scheduling configuration for tests
/// </summary>
public class SchedulingConfiguration : ISchedulingConfiguration
{
    public int SlotLengthMinutes => 30;
}
