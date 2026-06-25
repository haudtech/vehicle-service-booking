using FluentAssertions;
using Moq;
using VehicleServiceBooking.Application.Configuration.Interfaces;
using VehicleServiceBooking.Application.Interfaces;
using VehicleServiceBooking.Application.Interfaces.Persistence;
using VehicleServiceBooking.Application.Interfaces.Repositories;
using VehicleServiceBooking.Application.Interfaces.Services;
using VehicleServiceBooking.Application.Models;
using VehicleServiceBooking.Application.Services;
using VehicleServiceBooking.Domain.Entities;
using VehicleServiceBooking.Tests.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using Xunit;

namespace VehicleServiceBooking.Tests.Application.Services;

public class AvailabilityServiceTests
{
    private readonly Mock<IApplicationDbContext> _mockDbContext;
    private readonly Mock<ISchedulingConfiguration> _mockSchedulingConfiguration;
    private readonly Mock<ILogger<AvailabilityService>> _mockLogger;
    private readonly AvailabilityService _availabilityService;

    public AvailabilityServiceTests()
    {
        _mockDbContext = new Mock<IApplicationDbContext>();
        _mockSchedulingConfiguration = new Mock<ISchedulingConfiguration>();
        _mockLogger = new Mock<ILogger<AvailabilityService>>();

        // Default configuration: 30-minute slots
        _mockSchedulingConfiguration.Setup(c => c.SlotLengthMinutes).Returns(30);

        _availabilityService = new AvailabilityService(
            _mockSchedulingConfiguration.Object,
            _mockDbContext.Object
        );
    }

    #region GetAvailableSlotsAsync Tests

    [Fact]
    public async Task GetAvailableSlotsAsync_WithValidInputs_ShouldReturnAvailableSlots()
    {
        // Arrange
        var dealershipId = Guid.NewGuid();
        var serviceTypeId = Guid.NewGuid();
        var technicianId = Guid.NewGuid();
        var serviceBayId = Guid.NewGuid();
        var date = DateTime.UtcNow.AddDays(1).Date.AddHours(9); // Tomorrow at 9 AM

        var serviceType = ServiceTypeBuilder.ValidServiceType()
            .WithId(serviceTypeId)
            .StandardService()
            .Build();

        var technician = TechnicianBuilder.ValidTechnician()
            .WithId(technicianId)
            .WithDealershipId(dealershipId)
            .Build();

        var technicianSkill = TechnicianSkillBuilder.ValidSkill()
            .WithTechnicianId(technicianId)
            .WithServiceTypeId(serviceTypeId)
            .Build();

        var technicianSchedule = TechnicianScheduleBuilder.ValidSchedule()
            .WithTechnicianId(technicianId)
            .WithDayOfWeek(date.DayOfWeek)
            .StandardHours()
            .Build();

        var serviceBay = ServiceBayBuilder.ValidServiceBay()
            .WithId(serviceBayId)
            .WithDealershipId(dealershipId)
            .Build();

        // Setup mock DbContext
        SetupMockDbSet(serviceType, new List<ServiceType> { serviceType });
        SetupMockDbSet(technician, new List<Technician> { technician });
        SetupMockDbSet(technicianSkill, new List<TechnicianSkill> { technicianSkill });
        SetupMockDbSet(technicianSchedule, new List<TechnicianSchedule> { technicianSchedule });
        SetupMockDbSet(serviceBay, new List<ServiceBay> { serviceBay });
        SetupMockDbSet<Appointment>(null, new List<Appointment>());

        // Act
        var result = await _availabilityService.GetAvailableSlotsAsync(
            dealershipId,
            serviceTypeId,
            date,
            CancellationToken.None
        );

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        result.Should().AllSatisfy(slot =>
        {
            slot.TimeSlot.Should().NotBeNull();
            slot.TechnicianId.Should().Be(technicianId);
            slot.ServiceBayId.Should().Be(serviceBayId);
        });
    }

    [Fact]
    public async Task GetAvailableSlotsAsync_WithNoEligibleTechnicians_ShouldReturnEmptyList()
    {
        // Arrange
        var dealershipId = Guid.NewGuid();
        var serviceTypeId = Guid.NewGuid();
        var date = DateTime.UtcNow.AddDays(1).Date.AddHours(9);

        var serviceType = new ServiceType 
        { 
            Id = serviceTypeId, 
            Name = "Oil Change",
            DurationMinutes = 30 
        };

        // No technicians with this skill
        var technicianSkills = new List<TechnicianSkill>();

        // Setup mock DbContext
        SetupMockDbSet(serviceType, new List<ServiceType> { serviceType });
        SetupMockDbSet<Technician>(null, new List<Technician>());
        SetupMockDbSet<TechnicianSkill>(null, technicianSkills);
        SetupMockDbSet<TechnicianSchedule>(null, new List<TechnicianSchedule>());
        SetupMockDbSet<ServiceBay>(null, new List<ServiceBay>());
        SetupMockDbSet<Appointment>(null, new List<Appointment>());

        // Act
        var result = await _availabilityService.GetAvailableSlotsAsync(
            dealershipId,
            serviceTypeId,
            date,
            CancellationToken.None
        );

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAvailableSlotsAsync_WithTechnicianBusyDuringSlot_ShouldNotReturnThatSlot()
    {
        // Arrange
        var dealershipId = Guid.NewGuid();
        var serviceTypeId = Guid.NewGuid();
        var technicianId = Guid.NewGuid();
        var serviceBayId = Guid.NewGuid();
        var date = DateTime.UtcNow.AddDays(1).Date.AddHours(9); // Tomorrow at 9 AM

        var serviceType = ServiceTypeBuilder.ValidServiceType()
            .WithId(serviceTypeId)
            .StandardService()
            .Build();

        var technician = TechnicianBuilder.ValidTechnician()
            .WithId(technicianId)
            .WithDealershipId(dealershipId)
            .Build();

        var technicianSkill = TechnicianSkillBuilder.ValidSkill()
            .WithTechnicianId(technicianId)
            .WithServiceTypeId(serviceTypeId)
            .Build();

        var technicianSchedule = TechnicianScheduleBuilder.ValidSchedule()
            .WithTechnicianId(technicianId)
            .WithDayOfWeek(date.DayOfWeek)
            .StandardHours()
            .Build();

        var serviceBay = ServiceBayBuilder.ValidServiceBay()
            .WithId(serviceBayId)
            .WithDealershipId(dealershipId)
            .Build();

        // Existing appointment that blocks the first available slot
        var existingAppointment = AppointmentBuilder.ValidAppointment()
            .WithDealershipId(dealershipId)
            .WithTechnicianId(technicianId)
            .WithServiceBayId(serviceBayId)
            .WithTimeRange(date.Date.AddHours(9), date.Date.AddHours(9).AddMinutes(30))
            .Build();

        // Setup mock DbContext
        SetupMockDbSet(serviceType, new List<ServiceType> { serviceType });
        SetupMockDbSet(technician, new List<Technician> { technician });
        SetupMockDbSet(technicianSkill, new List<TechnicianSkill> { technicianSkill });
        SetupMockDbSet(technicianSchedule, new List<TechnicianSchedule> { technicianSchedule });
        SetupMockDbSet(serviceBay, new List<ServiceBay> { serviceBay });
        SetupMockDbSet(existingAppointment, new List<Appointment> { existingAppointment });

        // Act
        var result = await _availabilityService.GetAvailableSlotsAsync(
            dealershipId,
            serviceTypeId,
            date,
            CancellationToken.None
        );

        // Assert
        result.Should().NotBeNull();
        // Should have slots but not during the appointment time
        result.Should().AllSatisfy(slot =>
        {
            slot.TimeSlot.Start.Should().NotBe(date.Date.AddHours(9));
        });
    }

    [Fact]
    public async Task GetAvailableSlotsAsync_WithNoServiceBays_ShouldReturnEmptyList()
    {
        // Arrange
        var dealershipId = Guid.NewGuid();
        var serviceTypeId = Guid.NewGuid();
        var technicianId = Guid.NewGuid();
        var date = DateTime.UtcNow.AddDays(1).Date.AddHours(9);

        var serviceType = new ServiceType 
        { 
            Id = serviceTypeId, 
            Name = "Oil Change",
            DurationMinutes = 30 
        };

        var technician = new Technician 
        { 
            Id = technicianId, 
            DealershipId = dealershipId,
            FirstName = "John",
            LastName = "Doe"
        };

        var technicianSkill = new TechnicianSkill
        {
            Id = Guid.NewGuid(),
            TechnicianId = technicianId,
            ServiceTypeId = serviceTypeId
        };

        var technicianSchedule = new TechnicianSchedule
        {
            Id = Guid.NewGuid(),
            TechnicianId = technicianId,
            DayOfWeek = date.DayOfWeek,
            StartTime = new TimeOnly(8, 0),
            EndTime = new TimeOnly(17, 0)
        };

        // No service bays available
        var serviceBays = new List<ServiceBay>();

        // Setup mock DbContext
        SetupMockDbSet(serviceType, new List<ServiceType> { serviceType });
        SetupMockDbSet(technician, new List<Technician> { technician });
        SetupMockDbSet(technicianSkill, new List<TechnicianSkill> { technicianSkill });
        SetupMockDbSet(technicianSchedule, new List<TechnicianSchedule> { technicianSchedule });
        SetupMockDbSet<ServiceBay>(null, serviceBays);
        SetupMockDbSet<Appointment>(null, new List<Appointment>());

        // Act
        var result = await _availabilityService.GetAvailableSlotsAsync(
            dealershipId,
            serviceTypeId,
            date,
            CancellationToken.None
        );

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAvailableSlotsAsync_WithMultipleTechnicians_ShouldReturnOptionsForAll()
    {
        // Arrange
        var dealershipId = Guid.NewGuid();
        var serviceTypeId = Guid.NewGuid();
        var technician1Id = Guid.NewGuid();
        var technician2Id = Guid.NewGuid();
        var serviceBayId = Guid.NewGuid();
        var date = DateTime.UtcNow.AddDays(1).Date.AddHours(9);

        var serviceType = new ServiceType 
        { 
            Id = serviceTypeId, 
            Name = "Oil Change",
            DurationMinutes = 30 
        };

        var technician1 = new Technician 
        { 
            Id = technician1Id, 
            DealershipId = dealershipId,
            FirstName = "John",
            LastName = "Doe"
        };

        var technician2 = new Technician 
        { 
            Id = technician2Id, 
            DealershipId = dealershipId,
            FirstName = "Jane",
            LastName = "Smith"
        };

        var technicianSkill1 = new TechnicianSkill
        {
            Id = Guid.NewGuid(),
            TechnicianId = technician1Id,
            ServiceTypeId = serviceTypeId
        };

        var technicianSkill2 = new TechnicianSkill
        {
            Id = Guid.NewGuid(),
            TechnicianId = technician2Id,
            ServiceTypeId = serviceTypeId
        };

        var technicianSchedule1 = new TechnicianSchedule
        {
            Id = Guid.NewGuid(),
            TechnicianId = technician1Id,
            DayOfWeek = date.DayOfWeek,
            StartTime = new TimeOnly(8, 0),
            EndTime = new TimeOnly(17, 0)
        };

        var technicianSchedule2 = new TechnicianSchedule
        {
            Id = Guid.NewGuid(),
            TechnicianId = technician2Id,
            DayOfWeek = date.DayOfWeek,
            StartTime = new TimeOnly(8, 0),
            EndTime = new TimeOnly(17, 0)
        };

        var serviceBay = new ServiceBay
        {
            Id = serviceBayId,
            DealershipId = dealershipId,
            Name = "Bay 1"
        };

        // Setup mock DbContext
        SetupMockDbSet(serviceType, new List<ServiceType> { serviceType });
        SetupMockDbSet<Technician>(null, new List<Technician> { technician1, technician2 });
        SetupMockDbSet<TechnicianSkill>(null, new List<TechnicianSkill> { technicianSkill1, technicianSkill2 });
        SetupMockDbSet<TechnicianSchedule>(null, new List<TechnicianSchedule> { technicianSchedule1, technicianSchedule2 });
        SetupMockDbSet(serviceBay, new List<ServiceBay> { serviceBay });
        SetupMockDbSet<Appointment>(null, new List<Appointment>());

        // Act
        var result = await _availabilityService.GetAvailableSlotsAsync(
            dealershipId,
            serviceTypeId,
            date,
            CancellationToken.None
        );

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        
        var uniqueTechnicians = result.Select(r => r.TechnicianId).Distinct();
        uniqueTechnicians.Should().Contain(new[] { technician1Id, technician2Id });
    }

    [Fact]
    public async Task GetAvailableSlotsAsync_WithLongerServiceDuration_ShouldCalculateCorrectSlotCount()
    {
        // Arrange
        var dealershipId = Guid.NewGuid();
        var serviceTypeId = Guid.NewGuid();
        var technicianId = Guid.NewGuid();
        var serviceBayId = Guid.NewGuid();
        var date = DateTime.UtcNow.AddDays(1).Date.AddHours(9);

        // Service duration is 90 minutes (requires 3 slots of 30 minutes each)
        var serviceType = ServiceTypeBuilder.ValidServiceType()
            .WithId(serviceTypeId)
            .MajorService()
            .Build();

        var technician = TechnicianBuilder.ValidTechnician()
            .WithId(technicianId)
            .WithDealershipId(dealershipId)
            .Build();

        var technicianSkill = TechnicianSkillBuilder.ValidSkill()
            .WithTechnicianId(technicianId)
            .WithServiceTypeId(serviceTypeId)
            .Build();

        var technicianSchedule = TechnicianScheduleBuilder.ValidSchedule()
            .WithTechnicianId(technicianId)
            .WithDayOfWeek(date.DayOfWeek)
            .StandardHours()
            .Build();

        var serviceBay = ServiceBayBuilder.ValidServiceBay()
            .WithId(serviceBayId)
            .WithDealershipId(dealershipId)
            .Build();

        // Setup mock DbContext
        SetupMockDbSet(serviceType, new List<ServiceType> { serviceType });
        SetupMockDbSet(technician, new List<Technician> { technician });
        SetupMockDbSet(technicianSkill, new List<TechnicianSkill> { technicianSkill });
        SetupMockDbSet(technicianSchedule, new List<TechnicianSchedule> { technicianSchedule });
        SetupMockDbSet(serviceBay, new List<ServiceBay> { serviceBay });
        SetupMockDbSet<Appointment>(null, new List<Appointment>());

        // Act
        var result = await _availabilityService.GetAvailableSlotsAsync(
            dealershipId,
            serviceTypeId,
            date,
            CancellationToken.None
        );

        // Assert
        result.Should().NotBeNull();
        // Each slot should be 90 minutes long (3 slots of 30 minutes)
        result.Should().AllSatisfy(slot =>
        {
            var duration = slot.TimeSlot.End - slot.TimeSlot.Start;
            duration.Should().Be(TimeSpan.FromMinutes(90));
        });
    }

    [Fact]
    public async Task GetAvailableSlotsAsync_OutsideTechnicianSchedule_ShouldNotReturnSlot()
    {
        // Arrange
        var dealershipId = Guid.NewGuid();
        var serviceTypeId = Guid.NewGuid();
        var technicianId = Guid.NewGuid();
        var serviceBayId = Guid.NewGuid();
        var date = DateTime.UtcNow.AddDays(1).Date.AddHours(9);

        var serviceType = ServiceTypeBuilder.ValidServiceType()
            .WithId(serviceTypeId)
            .StandardService()
            .Build();

        var technician = TechnicianBuilder.ValidTechnician()
            .WithId(technicianId)
            .WithDealershipId(dealershipId)
            .Build();

        var technicianSkill = TechnicianSkillBuilder.ValidSkill()
            .WithTechnicianId(technicianId)
            .WithServiceTypeId(serviceTypeId)
            .Build();

        // Technician only works 10 AM to 2 PM
        var technicianSchedule = TechnicianScheduleBuilder.ValidSchedule()
            .WithTechnicianId(technicianId)
            .WithDayOfWeek(date.DayOfWeek)
            .RestrictedHours()
            .Build();

        var serviceBay = ServiceBayBuilder.ValidServiceBay()
            .WithId(serviceBayId)
            .WithDealershipId(dealershipId)
            .Build();

        // Setup mock DbContext
        SetupMockDbSet(serviceType, new List<ServiceType> { serviceType });
        SetupMockDbSet(technician, new List<Technician> { technician });
        SetupMockDbSet(technicianSkill, new List<TechnicianSkill> { technicianSkill });
        SetupMockDbSet(technicianSchedule, new List<TechnicianSchedule> { technicianSchedule });
        SetupMockDbSet(serviceBay, new List<ServiceBay> { serviceBay });
        SetupMockDbSet<Appointment>(null, new List<Appointment>());

        // Act
        var result = await _availabilityService.GetAvailableSlotsAsync(
            dealershipId,
            serviceTypeId,
            date,
            CancellationToken.None
        );

        // Assert
        result.Should().NotBeNull();
        // All slots should be between 10 AM and 2 PM
        result.Should().AllSatisfy(slot =>
        {
            slot.TimeSlot.Start.TimeOfDay.Should().BeGreaterThanOrEqualTo(new TimeSpan(10, 0, 0));
            slot.TimeSlot.End.TimeOfDay.Should().BeLessThanOrEqualTo(new TimeSpan(14, 0, 0));
        });
    }

    #endregion

    #region Helper Methods

    private void SetupMockDbSet<T>(T? singleEntity, IList<T> entityList) where T : class
    {
        var queryableList = entityList.AsQueryable();
        var mockDbSet = new Mock<DbSet<T>>();

        mockDbSet.As<IAsyncEnumerable<T>>()
            .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new TestAsyncEnumerator<T>(queryableList.GetEnumerator()));

        mockDbSet.As<IQueryable<T>>()
            .Setup(m => m.Provider)
            .Returns(new TestAsyncQueryProvider<T>(queryableList.Provider));

        mockDbSet.As<IQueryable<T>>()
            .Setup(m => m.Expression)
            .Returns(queryableList.Expression);

        mockDbSet.As<IQueryable<T>>()
            .Setup(m => m.ElementType)
            .Returns(queryableList.ElementType);

        mockDbSet.As<IQueryable<T>>()
            .Setup(m => m.GetEnumerator())
            .Returns(queryableList.GetEnumerator());

        if (singleEntity != null)
        {
            mockDbSet.Setup(m => m.Add(It.IsAny<T>()));
        }

        // Setup the DbContext property based on type
        if (typeof(T) == typeof(ServiceType))
        {
            _mockDbContext.Setup(m => m.ServiceTypes).Returns((DbSet<ServiceType>)(object)mockDbSet.Object);
        }
        else if (typeof(T) == typeof(Technician))
        {
            _mockDbContext.Setup(m => m.Technicians).Returns((DbSet<Technician>)(object)mockDbSet.Object);
        }
        else if (typeof(T) == typeof(TechnicianSkill))
        {
            _mockDbContext.Setup(m => m.TechnicianSkills).Returns((DbSet<TechnicianSkill>)(object)mockDbSet.Object);
        }
        else if (typeof(T) == typeof(TechnicianSchedule))
        {
            _mockDbContext.Setup(m => m.TechnicianSchedules).Returns((DbSet<TechnicianSchedule>)(object)mockDbSet.Object);
        }
        else if (typeof(T) == typeof(ServiceBay))
        {
            _mockDbContext.Setup(m => m.ServiceBays).Returns((DbSet<ServiceBay>)(object)mockDbSet.Object);
        }
        else if (typeof(T) == typeof(Appointment))
        {
            _mockDbContext.Setup(m => m.Appointments).Returns((DbSet<Appointment>)(object)mockDbSet.Object);
        }
    }

    #endregion
}
