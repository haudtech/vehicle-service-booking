using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VehicleServiceBooking.Application.Models.ViewModels;
using VehicleServiceBooking.Domain.Entities;
using VehicleServiceBooking.Domain.Enums;
using VehicleServiceBooking.Infrastructure.Persistence;
using VehicleServiceBooking.Tests.Common;

namespace VehicleServiceBooking.Tests.Infrastructure.Persistence.Views;

/// <summary>
/// Unit tests for Phase 3: Materialized Views
/// 
/// Tests verify that:
/// 1. Views are queryable from EF Core DbContext
/// 2. View results accurately reflect database state
/// 3. Conflict detection works correctly
/// 4. Dealership isolation is maintained
/// 5. Query performance meets expectations
/// </summary>
[TestClass]
public class TechnicianAvailableSlotsViewTests
{
    private ApplicationDbContext _context = null!;
    private Guid _dealershipId = Guid.NewGuid();
    private Guid _technicianId = Guid.NewGuid();
    private MaterializedViewsTestDataBuilder _dataBuilder = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"TechnicianAvailableSlotsTest_{Guid.NewGuid()}")
            .Options;

        _context = new ApplicationDbContext(options);
        _dataBuilder = MaterializedViewsTestDataBuilder.CreateValid()
            .WithDealershipId(_dealershipId)
            .WithTechnicianId(_technicianId);
        
        // Seed initial data
        SeedTestData();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context?.Dispose();
    }

    private void SeedTestData()
    {
        // Create dealership using builder
        var scenario = _dataBuilder.BuildCompleteScenario();
        _context.Dealerships.Add(scenario.Dealership);

        // Create technician using builder
        _context.Technicians.Add(scenario.Technician);

        // Create technician schedule
        var schedule = TechnicianScheduleBuilder.ValidSchedule()
            .WithTechnicianId(_technicianId)
            .StandardHours()
            .Build();
        _context.TechnicianSchedules.Add(schedule);

        _context.SaveChanges();
    }

    [TestMethod]
    public async Task QueryTechnicianAvailableSlotsView_WithoutConflicts_ReturnsAllSlots()
    {
        // Arrange: No services scheduled for this technician

        // Act
        var availableSlots = await _context.TechnicianAvailableSlotsView
            .Where(x => x.TechnicianId == _technicianId && x.DealershipId == _dealershipId)
            .ToListAsync();

        // Assert
        Assert.IsTrue(availableSlots.Count > 0, "Should return some available slots");
        Assert.IsTrue(availableSlots.All(x => x.IsAvailable), "All slots should be available without conflicts");
    }

    [TestMethod]
    public async Task QueryTechnicianAvailableSlotsView_WithConflictingService_ExcludesConflictingSlots()
    {
        // Arrange: Schedule a service for slots 1-2 using data builder
        var appointmentDate = DateOnly.FromDateTime(DateTime.Today);
        var slot1 = _context.TimeSlots.FirstOrDefault(x => x.SequenceOrder == 1)!;
        var slot2 = _context.TimeSlots.FirstOrDefault(x => x.SequenceOrder == 2)!;
        var slot3 = _context.TimeSlots.FirstOrDefault(x => x.SequenceOrder == 3)!;

        var appointment = _dataBuilder
            .BuildAppointment(appointmentDate);
        _context.Appointments.Add(appointment);

        var serviceType = _dataBuilder
            .WithServiceType("Oil Change", 60)
            .BuildCompleteScenario()
            .ServiceType;
        _context.ServiceTypes.Add(serviceType);

        var service = ServiceForViewTestBuilder.CreateValid()
            .WithAppointmentId(appointment.Id)
            .WithServiceTypeId(serviceType.Id)
            .WithTechnicianId(_technicianId)
            .WithDealershipId(_dealershipId)
            .WithTimeSlots(slot1.Id, slot2.Id)
            .Build();
        _context.Services.Add(service);
        _context.SaveChanges();

        // Act
        var availableSlots = await _context.TechnicianAvailableSlotsView
            .Where(x => x.TechnicianId == _technicianId && x.DealershipId == _dealershipId)
            .OrderBy(x => x.SequenceOrder)
            .ToListAsync();

        // Assert
        var slot1Result = availableSlots.FirstOrDefault(x => x.SequenceOrder == 1);
        var slot2Result = availableSlots.FirstOrDefault(x => x.SequenceOrder == 2);
        var slot3Result = availableSlots.FirstOrDefault(x => x.SequenceOrder == 3);

        Assert.IsNotNull(slot1Result, "Slot 1 should be in results");
        Assert.IsNotNull(slot2Result, "Slot 2 should be in results");
        Assert.IsNotNull(slot3Result, "Slot 3 should be in results");

        // Slot 1 and 2 should be unavailable (service occupies slots 1-2, exclusive of end)
        Assert.IsFalse(slot1Result.IsAvailable, "Slot 1 should be unavailable (service starts here)");
        Assert.IsFalse(slot2Result.IsAvailable, "Slot 2 should be unavailable (service ends here)");
        Assert.IsTrue(slot3Result.IsAvailable, "Slot 3 should be available (after service)");
    }

    [TestMethod]
    public async Task QueryTechnicianAvailableSlotsView_IncludesCorrectTechnicianDetails()
    {
        // Act
        var availableSlots = await _context.TechnicianAvailableSlotsView
            .Where(x => x.TechnicianId == _technicianId)
            .FirstOrDefaultAsync();

        // Assert
        Assert.IsNotNull(availableSlots);
        Assert.AreEqual(_technicianId, availableSlots.TechnicianId);
        Assert.AreEqual("John", availableSlots.FirstName);
        Assert.AreEqual("Doe", availableSlots.LastName);
        Assert.AreEqual(_dealershipId, availableSlots.DealershipId);
    }

    [TestMethod]
    public async Task QueryTechnicianAvailableSlotsView_IncludesTimeSlotDetails()
    {
        // Act
        var availableSlots = await _context.TechnicianAvailableSlotsView
            .Where(x => x.TechnicianId == _technicianId)
            .OrderBy(x => x.SequenceOrder)
            .Take(1)
            .ToListAsync();

        // Assert
        Assert.IsTrue(availableSlots.Count > 0);
        var firstSlot = availableSlots[0];
        
        Assert.AreEqual(1, firstSlot.SequenceOrder);
        Assert.AreEqual(new TimeOnly(8, 0), firstSlot.SlotStartTime);
        Assert.AreEqual(new TimeOnly(8, 30), firstSlot.SlotEndTime);
        Assert.IsNotNull(firstSlot.TimeSlotId);
    }
}

/// <summary>
/// Unit tests for ServiceBayAvailableSlots view
/// </summary>
[TestClass]
public class ServiceBayAvailableSlotsViewTests
{
    private ApplicationDbContext _context = null!;
    private Guid _dealershipId = Guid.NewGuid();
    private Guid _bayId = Guid.NewGuid();
    private MaterializedViewsTestDataBuilder _dataBuilder = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"ServiceBayAvailableSlotsTest_{Guid.NewGuid()}")
            .Options;

        _context = new ApplicationDbContext(options);
        _dataBuilder = MaterializedViewsTestDataBuilder.CreateValid()
            .WithDealershipId(_dealershipId)
            .WithServiceBayId(_bayId)
            .WithServiceBayName("Bay 1");
        
        SeedTestData();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context?.Dispose();
    }

    private void SeedTestData()
    {
        // Create dealership and bay using builder
        var scenario = _dataBuilder.BuildCompleteScenario();
        _context.Dealerships.Add(scenario.Dealership);
        _context.ServiceBays.Add(scenario.ServiceBay);
        _context.SaveChanges();
    }

    [TestMethod]
    public async Task QueryServiceBayAvailableSlotsView_WithoutConflicts_ReturnsAllSlots()
    {
        // Arrange: No services scheduled for this bay

        // Act
        var availableSlots = await _context.ServiceBayAvailableSlotsView
            .Where(x => x.ServiceBayId == _bayId && x.DealershipId == _dealershipId)
            .ToListAsync();

        // Assert
        Assert.IsTrue(availableSlots.Count > 0, "Should return available slots");
        Assert.IsTrue(availableSlots.All(x => x.IsAvailable), "All slots should be available");
    }

    [TestMethod]
    public async Task QueryServiceBayAvailableSlotsView_IncludesCorrectBayDetails()
    {
        // Act
        var availableSlot = await _context.ServiceBayAvailableSlotsView
            .Where(x => x.ServiceBayId == _bayId)
            .FirstOrDefaultAsync();

        // Assert
        Assert.IsNotNull(availableSlot);
        Assert.AreEqual(_bayId, availableSlot.ServiceBayId);
        Assert.AreEqual("Bay 1", availableSlot.ServiceBayName);
        Assert.AreEqual(_dealershipId, availableSlot.DealershipId);
    }

    [TestMethod]
    public async Task QueryServiceBayAvailableSlotsView_ExcludesInactiveBays()
    {
        // Arrange: Create inactive bay
        var inactiveBay = new ServiceBay
        {
            Id = Guid.NewGuid(),
            Name = "Bay 2 (Inactive)",
            DealershipId = _dealershipId,
            IsAvailable = true,
            IsActive = false, // INACTIVE
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.ServiceBays.Add(inactiveBay);
        _context.SaveChanges();

        // Act
        var availableSlots = await _context.ServiceBayAvailableSlotsView
            .Where(x => x.DealershipId == _dealershipId)
            .ToListAsync();

        // Assert
        Assert.IsFalse(availableSlots.Any(x => x.ServiceBayId == inactiveBay.Id),
            "View should exclude inactive bays");
    }
}

/// <summary>
/// Unit tests for ServiceTypeAvailability master view
/// </summary>
[TestClass]
public class ServiceTypeAvailabilityViewTests
{
    private ApplicationDbContext _context = null!;
    private Guid _dealershipId = Guid.NewGuid();
    private Guid _serviceTypeId = Guid.NewGuid();
    private Guid _technicianId = Guid.NewGuid();
    private Guid _bayId = Guid.NewGuid();
    private MaterializedViewsTestDataBuilder _dataBuilder = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"ServiceTypeAvailabilityTest_{Guid.NewGuid()}")
            .Options;

        _context = new ApplicationDbContext(options);
        _dataBuilder = MaterializedViewsTestDataBuilder.CreateValid()
            .WithDealershipId(_dealershipId)
            .WithServiceTypeId(_serviceTypeId)
            .WithTechnicianId(_technicianId)
            .WithServiceBayId(_bayId)
            .WithServiceType("Oil Change", 60)
            .WithTechnicianName("Jane", "Smith")
            .WithServiceBayName("Bay 1");

        SeedTestData();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context?.Dispose();
    }

    private void SeedTestData()
    {
        // Create all related entities using builder
        var scenario = _dataBuilder.BuildCompleteScenario();
        _context.Dealerships.Add(scenario.Dealership);
        _context.ServiceTypes.Add(scenario.ServiceType);
        _context.Technicians.Add(scenario.Technician);
        _context.ServiceBays.Add(scenario.ServiceBay);

        // Create technician skill
        var skill = TechnicianSkillBuilder.ValidSkill()
            .WithTechnicianId(_technicianId)
            .WithServiceTypeId(_serviceTypeId)
            .Build();
        _context.TechnicianSkills.Add(skill);

        // Create technician schedule
        var schedule = TechnicianScheduleBuilder.ValidSchedule()
            .WithTechnicianId(_technicianId)
            .StandardHours()
            .Build();
        _context.TechnicianSchedules.Add(schedule);

        _context.SaveChanges();
    }

    [TestMethod]
    public async Task QueryServiceTypeAvailabilityView_WithoutConflicts_ReturnsOptions()
    {
        // Arrange: No services scheduled

        // Act
        var availability = await _context.ServiceTypeAvailabilityView
            .Where(x => x.ServiceTypeId == _serviceTypeId &&
                        x.DealershipId == _dealershipId &&
                        x.CanFitService)
            UpdatedAt = DateTime.UtcNow
        };
        _context.TechnicianSkills.Add(skill);

        // Create technician schedule
        var schedule = new TechnicianSchedule
        {
            Id = Guid.NewGuid(),
            TechnicianId = _technicianId,
            DayOfWeek = 2, // Monday
            StartTime = new TimeOnly(8, 0),
            EndTime = new TimeOnly(17, 0),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.TechnicianSchedules.Add(schedule);

        // Create service bay
        var bay = new ServiceBay
        {
            Id = _bayId,
            Name = "Bay 1",
            DealershipId = _dealershipId,
            IsAvailable = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.ServiceBays.Add(bay);

        _context.SaveChanges();
    }

    [TestMethod]
    public async Task QueryServiceTypeAvailabilityView_WithoutConflicts_ReturnsOptions()
    {
        // Arrange: No services scheduled

        // Act
        var availability = await _context.ServiceTypeAvailabilityView
            .Where(x => x.ServiceTypeId == _serviceTypeId && 
                        x.DealershipId == _dealershipId &&
                        x.CanFitService)
            .ToListAsync();

        // Assert
        Assert.IsTrue(availability.Count > 0, "Should return availability options");
        Assert.IsTrue(availability.All(x => x.CanFitService), "All results should have CanFitService = true");
    }

    [TestMethod]
    public async Task QueryServiceTypeAvailabilityView_CalculatesRequiredSlots()
    {
        // Act
        var availability = await _context.ServiceTypeAvailabilityView
            .Where(x => x.ServiceTypeId == _serviceTypeId)
            .FirstOrDefaultAsync();

        // Assert
        Assert.IsNotNull(availability);
        Assert.AreEqual(60, availability.DurationMinutes);
        Assert.AreEqual(2, availability.RequiredSlots); // 60 minutes = 2 × 30-min slots
    }

    [TestMethod]
    public async Task QueryServiceTypeAvailabilityView_IncludesTechnicianDetails()
    {
        // Act
        var availability = await _context.ServiceTypeAvailabilityView
            .Where(x => x.ServiceTypeId == _serviceTypeId && x.TechnicianId == _technicianId)
            .FirstOrDefaultAsync();

        // Assert
        Assert.IsNotNull(availability);
        Assert.AreEqual(_technicianId, availability.TechnicianId);
        Assert.AreEqual("Jane", availability.FirstName);
        Assert.AreEqual("Smith", availability.LastName);
    }

    [TestMethod]
    public async Task QueryServiceTypeAvailabilityView_IncludesBayDetails()
    {
        // Act
        var availability = await _context.ServiceTypeAvailabilityView
            .Where(x => x.ServiceTypeId == _serviceTypeId && x.ServiceBayId == _bayId)
            .FirstOrDefaultAsync();

        // Assert
        Assert.IsNotNull(availability);
        Assert.AreEqual(_bayId, availability.ServiceBayId);
        Assert.AreEqual("Bay 1", availability.ServiceBayName);
    }

    [TestMethod]
    public async Task QueryServiceTypeAvailabilityView_FiltersByServiceTypeSkill()
    {
        // Arrange: Create service type without technician skill
        var unskilledServiceType = new ServiceType
        {
            Id = Guid.NewGuid(),
            DealershipId = _dealershipId,
            Name = "Advanced Repair",
            DurationMinutes = 120,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.ServiceTypes.Add(unskilledServiceType);
        _context.SaveChanges();

        // Act
        var availability = await _context.ServiceTypeAvailabilityView
            .Where(x => x.ServiceTypeId == unskilledServiceType.Id && 
                        x.TechnicianId == _technicianId)
            .ToListAsync();

        // Assert
        Assert.AreEqual(0, availability.Count, 
            "Technician without skill should not be in availability results");
    }
}
