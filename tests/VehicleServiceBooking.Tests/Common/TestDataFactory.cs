using VehicleServiceBooking.Application.DTOs;
using VehicleServiceBooking.Domain.Entities;
using VehicleServiceBooking.Domain.Enums;

namespace VehicleServiceBooking.Tests.Common;

/// <summary>
/// Centralized factory for creating complete test scenarios with related entities.
/// Reduces duplication across test files by providing pre-configured entity sets.
/// </summary>
public static class TestDataFactory
{
    /// <summary>
    /// Creates a complete appointment service request scenario with all related entities
    /// </summary>
    public static CreateAppointmentScenario CreateAppointmentWithAllEntities(
        Guid? dealershipId = null,
        Guid? customerId = null,
        Guid? vehicleId = null,
        Guid? serviceTypeId = null,
        Guid? technicianId = null,
        Guid? serviceBayId = null,
        DateTime? slotStart = null)
    {
        dealershipId ??= Guid.NewGuid();
        customerId ??= Guid.NewGuid();
        vehicleId ??= Guid.NewGuid();
        serviceTypeId ??= Guid.NewGuid();
        technicianId ??= Guid.NewGuid();
        serviceBayId ??= Guid.NewGuid();
        slotStart ??= DateTime.UtcNow.AddHours(1);
        var slotEnd = slotStart.Value.AddHours(1);

        var dealership = DealershipBuilder.ValidDealership()
            .WithId(dealershipId.Value)
            .Build();

        var customer = CustomerBuilder.ValidCustomer()
            .WithId(customerId.Value)
            .Build();

        var vehicle = VehicleBuilder.ValidVehicle()
            .WithId(vehicleId.Value)
            .WithCustomerId(customerId.Value)
            .Build();

        var serviceType = ServiceTypeBuilder.ValidServiceType()
            .WithId(serviceTypeId.Value)
            .Build();

        var technician = TechnicianBuilder.ValidTechnician()
            .WithId(technicianId.Value)
            .WithDealershipId(dealershipId.Value)
            .Build();

        var serviceBay = ServiceBayBuilder.ValidServiceBay()
            .WithId(serviceBayId.Value)
            .WithDealershipId(dealershipId.Value)
            .Build();

        var request = new CreateAppointmentRequest
        {
            DealershipId = dealershipId.Value,
            CustomerId = customerId.Value,
            VehicleId = vehicleId.Value,
            ServiceTypeId = serviceTypeId.Value,
            TechnicianId = technicianId.Value,
            ServiceBayId = serviceBayId.Value,
            SlotStart = slotStart.Value,
            SlotEnd = slotEnd
        };

        return new CreateAppointmentScenario
        {
            Request = request,
            Dealership = dealership,
            Customer = customer,
            Vehicle = vehicle,
            ServiceType = serviceType,
            Technician = technician,
            ServiceBay = serviceBay,
            SlotStart = slotStart.Value,
            SlotEnd = slotEnd
        };
    }

    /// <summary>
    /// Creates an appointment status lookup for testing
    /// </summary>
    public static AppointmentStatusLookup CreateAppointmentStatus(
        AppointmentStatus status = AppointmentStatus.Booked)
    {
        return status switch
        {
            AppointmentStatus.Booked => new AppointmentStatusLookup
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Status = AppointmentStatus.Booked,
                Name = "Booked",
                Description = "Appointment is scheduled"
            },
            AppointmentStatus.InProgress => new AppointmentStatusLookup
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                Status = AppointmentStatus.InProgress,
                Name = "In Progress",
                Description = "Service is in progress"
            },
            AppointmentStatus.Completed => new AppointmentStatusLookup
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                Status = AppointmentStatus.Completed,
                Name = "Completed",
                Description = "Service has been completed"
            },
            AppointmentStatus.Cancelled => new AppointmentStatusLookup
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000004"),
                Status = AppointmentStatus.Cancelled,
                Name = "Cancelled",
                Description = "Appointment was cancelled"
            },
            _ => new AppointmentStatusLookup
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Status = AppointmentStatus.Booked,
                Name = "Booked",
                Description = "Appointment is scheduled"
            }
        };
    }

    /// <summary>
    /// Creates an appointment with services collection
    /// </summary>
    public static Appointment CreateAppointmentWithService(
        DateTime slotStart,
        DateTime slotEnd,
        Guid? serviceBayId = null,
        Guid? technicianId = null,
        Guid? serviceTypeId = null,
        Guid? appointmentId = null,
        Guid? dealershipId = null)
    {
        appointmentId ??= Guid.NewGuid();
        dealershipId ??= Guid.NewGuid();
        serviceBayId ??= Guid.NewGuid();
        technicianId ??= Guid.NewGuid();
        serviceTypeId ??= Guid.NewGuid();

        var bookedStatus = CreateAppointmentStatus(AppointmentStatus.Booked);

        return new Appointment
        {
            Id = appointmentId.Value,
            DealershipId = dealershipId.Value,
            CustomerId = Guid.NewGuid(),
            VehicleId = Guid.NewGuid(),
            AppointmentDate = DateOnly.FromDateTime(slotStart),
            StatusId = bookedStatus.Id,
            Status = bookedStatus,
            Services = new List<Service>
            {
                new Service
                {
                    Id = Guid.NewGuid(),
                    AppointmentId = appointmentId.Value,
                    ServiceTypeId = serviceTypeId.Value,
                    TechnicianId = technicianId.Value,
                    ServiceBayId = serviceBayId.Value,
                    DealershipId = dealershipId.Value,
                    ServiceStatusId = Guid.Parse("00000000-0000-0000-0000-000000000101"), // Pending
                    SequenceOrder = 1,
                    EstimatedStartTime = slotStart,
                    EstimatedEndTime = slotEnd
                }
            }
        };
    }

    /// <summary>
    /// Creates multiple appointments for conflict testing
    /// </summary>
    public static (Appointment ConflictingAppointment, Appointment NonConflictingAppointment) 
        CreateAppointmentPair(
        DateTime baseTime,
        Guid dealershipId,
        Guid serviceBayId,
        Guid technicianId,
        Guid serviceTypeId)
    {
        var conflictingAppointment = CreateAppointmentWithService(
            slotStart: baseTime.AddMinutes(-30),
            slotEnd: baseTime.AddHours(1).AddMinutes(30),
            serviceBayId: serviceBayId,
            technicianId: technicianId,
            serviceTypeId: serviceTypeId,
            dealershipId: dealershipId
        );

        var nonConflictingAppointment = CreateAppointmentWithService(
            slotStart: baseTime.AddHours(3),
            slotEnd: baseTime.AddHours(4),
            serviceBayId: serviceBayId,
            technicianId: technicianId,
            serviceTypeId: serviceTypeId,
            dealershipId: dealershipId
        );

        return (conflictingAppointment, nonConflictingAppointment);
    }

    /// <summary>
    /// Creates a complete appointment scenario for conflict detection testing
    /// </summary>
    public static ConflictDetectionScenario CreateConflictDetectionScenario()
    {
        var dealershipId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var vehicleId = Guid.NewGuid();
        var serviceTypeId = Guid.NewGuid();
        var technicianId = Guid.NewGuid();
        var serviceBayId = Guid.NewGuid();
        var startTime = DateTime.UtcNow.AddHours(1);
        var endTime = startTime.AddHours(1);

        var request = new CreateAppointmentRequest
        {
            DealershipId = dealershipId,
            CustomerId = customerId,
            VehicleId = vehicleId,
            ServiceTypeId = serviceTypeId,
            TechnicianId = technicianId,
            ServiceBayId = serviceBayId,
            SlotStart = startTime,
            SlotEnd = endTime
        };

        var dealership = DealershipBuilder.ValidDealership()
            .WithId(dealershipId)
            .Build();

        var customer = CustomerBuilder.ValidCustomer()
            .WithId(customerId)
            .Build();

        var vehicle = VehicleBuilder.ValidVehicle()
            .WithId(vehicleId)
            .WithCustomerId(customerId)
            .Build();

        var serviceType = ServiceTypeBuilder.ValidServiceType()
            .WithId(serviceTypeId)
            .Build();

        var technician = TechnicianBuilder.ValidTechnician()
            .WithId(technicianId)
            .WithDealershipId(dealershipId)
            .Build();

        var serviceBay = ServiceBayBuilder.ValidServiceBay()
            .WithId(serviceBayId)
            .WithDealershipId(dealershipId)
            .Build();

        var (conflictingAppointment, nonConflictingAppointment) = 
            CreateAppointmentPair(startTime, dealershipId, serviceBayId, technicianId, serviceTypeId);

        return new ConflictDetectionScenario
        {
            Request = request,
            Dealership = dealership,
            Customer = customer,
            Vehicle = vehicle,
            ServiceType = serviceType,
            Technician = technician,
            ServiceBay = serviceBay,
            ConflictingAppointment = conflictingAppointment,
            NonConflictingAppointment = nonConflictingAppointment,
            SlotStart = startTime,
            SlotEnd = endTime
        };
    }
}

/// <summary>
/// Represents a complete appointment service request scenario
/// </summary>
public class CreateAppointmentScenario
{
    public CreateAppointmentRequest Request { get; set; } = null!;
    public Dealership Dealership { get; set; } = null!;
    public Customer Customer { get; set; } = null!;
    public Vehicle Vehicle { get; set; } = null!;
    public ServiceType ServiceType { get; set; } = null!;
    public Technician Technician { get; set; } = null!;
    public ServiceBay ServiceBay { get; set; } = null!;
    public DateTime SlotStart { get; set; }
    public DateTime SlotEnd { get; set; }
}

/// <summary>
/// Represents a conflict detection test scenario
/// </summary>
public class ConflictDetectionScenario
{
    public CreateAppointmentRequest Request { get; set; } = null!;
    public Dealership Dealership { get; set; } = null!;
    public Customer Customer { get; set; } = null!;
    public Vehicle Vehicle { get; set; } = null!;
    public ServiceType ServiceType { get; set; } = null!;
    public Technician Technician { get; set; } = null!;
    public ServiceBay ServiceBay { get; set; } = null!;
    public Appointment ConflictingAppointment { get; set; } = null!;
    public Appointment NonConflictingAppointment { get; set; } = null!;
    public DateTime SlotStart { get; set; }
    public DateTime SlotEnd { get; set; }
}
