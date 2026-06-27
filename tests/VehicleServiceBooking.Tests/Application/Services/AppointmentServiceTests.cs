using FluentAssertions;
using Moq;
using VehicleServiceBooking.Application.DTOs;
using VehicleServiceBooking.Application.Interfaces;
using VehicleServiceBooking.Application.Interfaces.Persistence;
using VehicleServiceBooking.Application.Interfaces.Repositories;
using VehicleServiceBooking.Application.Interfaces.Services;
using VehicleServiceBooking.Application.Services;
using VehicleServiceBooking.Domain.Entities;
using VehicleServiceBooking.Domain.Enums;
using VehicleServiceBooking.Infrastructure.Persistence;
using VehicleServiceBooking.Tests.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using Xunit;

namespace VehicleServiceBooking.Tests.Application.Services;

public class AppointmentServiceTests
{
    private readonly Mock<IApplicationDbContext> _mockDbContext;
    private readonly Mock<IAppointmentRepository> _mockAppointmentRepository;
    private readonly Mock<IAvailabilityService> _mockAvailabilityService;
    private readonly Mock<ILogger<AppointmentService>> _mockLogger;
    private readonly AppointmentService _appointmentService;

    public AppointmentServiceTests()
    {
        _mockDbContext = new Mock<IApplicationDbContext>();
        _mockAppointmentRepository = new Mock<IAppointmentRepository>();
        _mockAvailabilityService = new Mock<IAvailabilityService>();
        _mockLogger = new Mock<ILogger<AppointmentService>>();

        _appointmentService = new AppointmentService(
            _mockAppointmentRepository.Object,
            _mockAvailabilityService.Object,
            _mockLogger.Object
        );
    }

    #region CreateAppointmentAsync Tests

    [Fact]
    public async Task CreateAppointmentAsync_WithValidRequest_ShouldCreateAppointment()
    {
        // Arrange
        var scenario = TestDataFactory.CreateAppointmentWithAllEntities();
        var bookedStatus = TestDataFactory.CreateAppointmentStatus(AppointmentStatus.Booked);

        SetupMockDbSet(scenario.Dealership, new List<Dealership> { scenario.Dealership });
        SetupMockDbSet(scenario.Customer, new List<Customer> { scenario.Customer });
        SetupMockDbSet(scenario.Vehicle, new List<Vehicle> { scenario.Vehicle });
        SetupMockDbSet(scenario.ServiceType, new List<ServiceType> { scenario.ServiceType });
        SetupMockDbSet(scenario.Technician, new List<Technician> { scenario.Technician });
        SetupMockDbSet(scenario.ServiceBay, new List<ServiceBay> { scenario.ServiceBay });
        SetupMockDbSet(bookedStatus, new List<AppointmentStatusLookup> { bookedStatus });
        SetupMockDbSet<Appointment>(null, new List<Appointment>());

        _mockDbContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Setup repository to return the appointment when AddAsync is called
        _mockAppointmentRepository
            .Setup(r => r.AddAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Appointment app, CancellationToken ct) => app);

        // Act
        var result = await _appointmentService.CreateAppointmentAsync(scenario.Request);

        // Assert
        result.Should().NotBeNull();
        result.AppointmentId.Should().NotBeEmpty();
        result.SlotStart.Should().Be(scenario.SlotStart);
        result.SlotEnd.Should().Be(scenario.SlotEnd);
    }

    [Fact]
    public async Task CreateAppointmentAsync_WithNonExistentCustomer_ShouldThrowException()
    {
        // Arrange
        var scenario = TestDataFactory.CreateAppointmentWithAllEntities();

        // Setup mocks - empty lists to simulate non-existent entities
        SetupMockDbSet<Dealership>(null, new List<Dealership>());
        SetupMockDbSet<Customer>(null, new List<Customer>());
        SetupMockDbSet<Vehicle>(null, new List<Vehicle>());
        SetupMockDbSet<ServiceType>(null, new List<ServiceType>());
        SetupMockDbSet<Technician>(null, new List<Technician>());
        SetupMockDbSet<ServiceBay>(null, new List<ServiceBay>());
        SetupMockDbSet<Appointment>(null, new List<Appointment>());

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _appointmentService.CreateAppointmentAsync(scenario.Request)
        );
    }

    [Fact]
    public async Task CreateAppointmentAsync_WithNonExistentVehicle_ShouldThrowException()
    {
        // Arrange
        var scenario = TestDataFactory.CreateAppointmentWithAllEntities();

        // Setup mocks
        SetupMockDbSet(scenario.Dealership, new List<Dealership> { scenario.Dealership });
        SetupMockDbSet(scenario.Customer, new List<Customer> { scenario.Customer });
        SetupMockDbSet<Vehicle>(null, new List<Vehicle>()); // Empty - no vehicles
        SetupMockDbSet(scenario.ServiceType, new List<ServiceType> { scenario.ServiceType });
        SetupMockDbSet(scenario.Technician, new List<Technician> { scenario.Technician });
        SetupMockDbSet(scenario.ServiceBay, new List<ServiceBay> { scenario.ServiceBay });
        
        var bookedStatus = TestDataFactory.CreateAppointmentStatus(AppointmentStatus.Booked);
        SetupMockDbSet(bookedStatus, new List<AppointmentStatusLookup> { bookedStatus });
        
        SetupMockDbSet<Appointment>(null, new List<Appointment>());

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _appointmentService.CreateAppointmentAsync(scenario.Request)
        );
    }

    [Fact]
    public async Task CreateAppointmentAsync_WithConflictingSlot_ShouldThrowException()
    {
        // Arrange
        var scenario = TestDataFactory.CreateAppointmentWithAllEntities();
        var conflictScenario = TestDataFactory.CreateConflictDetectionScenario();
        var bookedStatus = TestDataFactory.CreateAppointmentStatus(AppointmentStatus.Booked);

        // Use the conflicting appointment from the scenario
        var existingAppointment = conflictScenario.ConflictingAppointment;

        SetupMockDbSet(scenario.Dealership, new List<Dealership> { scenario.Dealership });
        SetupMockDbSet(scenario.Customer, new List<Customer> { scenario.Customer });
        SetupMockDbSet(scenario.Vehicle, new List<Vehicle> { scenario.Vehicle });
        SetupMockDbSet(scenario.ServiceType, new List<ServiceType> { scenario.ServiceType });
        SetupMockDbSet(scenario.Technician, new List<Technician> { scenario.Technician });
        SetupMockDbSet(scenario.ServiceBay, new List<ServiceBay> { scenario.ServiceBay });
        SetupMockDbSet(bookedStatus, new List<AppointmentStatusLookup> { bookedStatus });
        SetupMockDbSet(existingAppointment, new List<Appointment> { existingAppointment });

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _appointmentService.CreateAppointmentAsync(scenario.Request)
        );
    }

    [Fact]
    public async Task CreateAppointmentAsync_WithNonConflictingExistingAppointment_ShouldSucceed()
    {
        // Arrange
        var scenario = TestDataFactory.CreateAppointmentWithAllEntities();
        var conflictScenario = TestDataFactory.CreateConflictDetectionScenario();
        var bookedStatus = TestDataFactory.CreateAppointmentStatus(AppointmentStatus.Booked);

        // Use the non-conflicting appointment from the scenario
        var existingAppointment = conflictScenario.NonConflictingAppointment;

        SetupMockDbSet(scenario.Dealership, new List<Dealership> { scenario.Dealership });
        SetupMockDbSet(scenario.Customer, new List<Customer> { scenario.Customer });
        SetupMockDbSet(scenario.Vehicle, new List<Vehicle> { scenario.Vehicle });
        SetupMockDbSet(scenario.ServiceType, new List<ServiceType> { scenario.ServiceType });
        SetupMockDbSet(scenario.Technician, new List<Technician> { scenario.Technician });
        SetupMockDbSet(scenario.ServiceBay, new List<ServiceBay> { scenario.ServiceBay });
        SetupMockDbSet(bookedStatus, new List<AppointmentStatusLookup> { bookedStatus });
        SetupMockDbSet(existingAppointment, new List<Appointment> { existingAppointment });

        _mockDbContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _appointmentService.CreateAppointmentAsync(scenario.Request);

        // Assert
        result.Should().NotBeNull();
        result.AppointmentId.Should().NotBeEmpty();
        result.SlotStart.Should().Be(scenario.SlotStart);
        result.SlotEnd.Should().Be(scenario.SlotEnd);
    }

    #endregion

    #region GetAppointmentByIdAsync Tests

    [Fact]
    public async Task GetAppointmentByIdAsync_WithValidId_ShouldReturnAppointment()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var startTime = DateTime.UtcNow.AddHours(1);
        var endTime = startTime.AddHours(1);
        
        var bookedStatus = TestDataFactory.CreateAppointmentStatus(AppointmentStatus.Booked);
        var appointment = TestDataFactory.CreateAppointmentWithService(startTime, endTime, appointmentId: appointmentId);

        SetupMockDbSet(bookedStatus, new List<AppointmentStatusLookup> { bookedStatus });
        SetupMockDbSet(appointment, new List<Appointment> { appointment });

        // Act
        var result = await _appointmentService.GetAppointmentByIdAsync(appointmentId);

        // Assert
        result.Should().NotBeNull();
        result.AppointmentId.Should().Be(appointmentId);
        result.SlotStart.Should().Be(startTime);
        result.SlotEnd.Should().Be(endTime);
    }

    [Fact]
    public async Task GetAppointmentByIdAsync_WithInvalidId_ShouldThrowException()
    {
        // Arrange
        var invalidId = Guid.NewGuid();
        SetupMockDbSet<Appointment>(null, new List<Appointment>());

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _appointmentService.GetAppointmentByIdAsync(invalidId)
        );
    }

    [Fact]
    public async Task GetAppointmentByIdAsync_WithMultipleAppointments_ShouldReturnCorrectOne()
    {
        // Arrange
        var targetId = Guid.NewGuid();
        var startTime1 = DateTime.UtcNow.AddHours(1);
        var startTime2 = DateTime.UtcNow.AddHours(3);
        
        var bookedStatus = TestDataFactory.CreateAppointmentStatus(AppointmentStatus.Booked);
        var completedStatus = TestDataFactory.CreateAppointmentStatus(AppointmentStatus.Completed);
        
        var appointment1 = TestDataFactory.CreateAppointmentWithService(startTime1, startTime1.AddHours(1), appointmentId: targetId);
        var appointment2 = TestDataFactory.CreateAppointmentWithService(startTime2, startTime2.AddHours(1));

        SetupMockDbSet(bookedStatus, new List<AppointmentStatusLookup> { bookedStatus });
        SetupMockDbSet(completedStatus, new List<AppointmentStatusLookup> { completedStatus });
        SetupMockDbSet(null, new List<Appointment> { appointment1, appointment2 });

        // Act
        var result = await _appointmentService.GetAppointmentByIdAsync(targetId);

        // Assert
        result.Should().NotBeNull();
        result.AppointmentId.Should().Be(targetId);
        result.SlotStart.Should().Be(startTime1);
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
        if (typeof(T) == typeof(Dealership))
        {
            _mockDbContext.Setup(m => m.Dealerships).Returns((DbSet<Dealership>)(object)mockDbSet.Object);
        }
        else if (typeof(T) == typeof(Customer))
        {
            _mockDbContext.Setup(m => m.Customers).Returns((DbSet<Customer>)(object)mockDbSet.Object);
        }
        else if (typeof(T) == typeof(Vehicle))
        {
            _mockDbContext.Setup(m => m.Vehicles).Returns((DbSet<Vehicle>)(object)mockDbSet.Object);
        }
        else if (typeof(T) == typeof(ServiceType))
        {
            _mockDbContext.Setup(m => m.ServiceTypes).Returns((DbSet<ServiceType>)(object)mockDbSet.Object);
        }
        else if (typeof(T) == typeof(Technician))
        {
            _mockDbContext.Setup(m => m.Technicians).Returns((DbSet<Technician>)(object)mockDbSet.Object);
        }
        else if (typeof(T) == typeof(ServiceBay))
        {
            _mockDbContext.Setup(m => m.ServiceBays).Returns((DbSet<ServiceBay>)(object)mockDbSet.Object);
        }
        else if (typeof(T) == typeof(AppointmentStatusLookup))
        {
            _mockDbContext.Setup(m => m.AppointmentStatusLookups).Returns((DbSet<AppointmentStatusLookup>)(object)mockDbSet.Object);
        }
        else if (typeof(T) == typeof(Appointment))
        {
            _mockDbContext.Setup(m => m.Appointments).Returns((DbSet<Appointment>)(object)mockDbSet.Object);
        }
    }

    #endregion
}

#region Test Helpers for Async LINQ

public class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> _inner;

    public TestAsyncEnumerator(IEnumerator<T> inner)
    {
        _inner = inner;
    }

    public T Current => _inner.Current;

    public ValueTask DisposeAsync()
    {
        _inner.Dispose();
        return ValueTask.CompletedTask;
    }

    public ValueTask<bool> MoveNextAsync()
    {
        return ValueTask.FromResult(_inner.MoveNext());
    }
}

public class TestAsyncQueryProvider<T> : IQueryProvider
{
    private readonly IQueryProvider _inner;

    public TestAsyncQueryProvider(IQueryProvider inner)
    {
        _inner = inner;
    }

    public IQueryable CreateQuery(Expression expression)
    {
        return new TestAsyncQuery<T>(expression);
    }

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        return new TestAsyncQuery<TElement>(expression);
    }

    public object Execute(Expression expression)
    {
        return _inner.Execute(expression);
    }

    public TResult Execute<TResult>(Expression expression)
    {
        return _inner.Execute<TResult>(expression);
    }
}

public class TestAsyncQuery<T> : IAsyncEnumerable<T>, IQueryable<T>
{
    private readonly IQueryable<T> _inner;

    public TestAsyncQuery(IQueryable<T> inner)
    {
        _inner = inner;
    }

    public TestAsyncQuery(Expression expression)
    {
        _inner = Enumerable.Empty<T>().AsQueryable();
    }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return new TestAsyncEnumerator<T>(_inner.GetEnumerator());
    }

    Expression IQueryable.Expression => _inner.Expression;
    public Type ElementType => typeof(T);
    public IQueryProvider Provider => new TestAsyncQueryProvider<T>(_inner.Provider);

    public IEnumerator<T> GetEnumerator()
    {
        return _inner.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

#endregion
