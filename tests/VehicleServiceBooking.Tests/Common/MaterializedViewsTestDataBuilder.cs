using System;
using System.Collections.Generic;
using VehicleServiceBooking.Application.Models.ViewModels;
using VehicleServiceBooking.Domain.Entities;

namespace VehicleServiceBooking.Tests.Common;

/// <summary>
/// Test data builder for materialalized views tests.
/// Provides fluent API to create complete test scenarios for TechnicianAvailableSlots,
/// ServiceBayAvailableSlots, and ServiceTypeAvailability views.
/// </summary>
public class MaterializedViewsTestDataBuilder
{
    private Guid _dealershipId = Guid.NewGuid();
    private Guid _technicianId = Guid.NewGuid();
    private Guid _serviceTypeId = Guid.NewGuid();
    private Guid _serviceBayId = Guid.NewGuid();
    private DateOnly _queryDate = DateOnly.FromDateTime(DateTime.Today);
    
    private string _technicianFirstName = "John";
    private string _technicianLastName = "Doe";
    private string _serviceBayName = "Bay 1";
    private string _serviceTypeName = "Oil Change";
    private int _durationMinutes = 60;
    
    private TimeOnly _slotStartTime = new TimeOnly(8, 0);
    private TimeOnly _slotEndTime = new TimeOnly(8, 30);
    private int _sequenceOrder = 1;
    private bool _isAvailable = true;

    /// <summary>
    /// Creates a new builder with default test values
    /// </summary>
    public static MaterializedViewsTestDataBuilder CreateValid()
    {
        return new MaterializedViewsTestDataBuilder();
    }

    /// <summary>
    /// Sets the dealership ID for all entities
    /// </summary>
    public MaterializedViewsTestDataBuilder WithDealershipId(Guid dealershipId)
    {
        _dealershipId = dealershipId;
        return this;
    }

    /// <summary>
    /// Sets the technician ID
    /// </summary>
    public MaterializedViewsTestDataBuilder WithTechnicianId(Guid technicianId)
    {
        _technicianId = technicianId;
        return this;
    }

    /// <summary>
    /// Sets technician name
    /// </summary>
    public MaterializedViewsTestDataBuilder WithTechnicianName(string firstName, string lastName)
    {
        _technicianFirstName = firstName;
        _technicianLastName = lastName;
        return this;
    }

    /// <summary>
    /// Sets the service type ID
    /// </summary>
    public MaterializedViewsTestDataBuilder WithServiceTypeId(Guid serviceTypeId)
    {
        _serviceTypeId = serviceTypeId;
        return this;
    }

    /// <summary>
    /// Sets service type name and duration
    /// </summary>
    public MaterializedViewsTestDataBuilder WithServiceType(string name, int durationMinutes)
    {
        _serviceTypeName = name;
        _durationMinutes = durationMinutes;
        return this;
    }

    /// <summary>
    /// Sets the service bay ID
    /// </summary>
    public MaterializedViewsTestDataBuilder WithServiceBayId(Guid serviceBayId)
    {
        _serviceBayId = serviceBayId;
        return this;
    }

    /// <summary>
    /// Sets service bay name
    /// </summary>
    public MaterializedViewsTestDataBuilder WithServiceBayName(string name)
    {
        _serviceBayName = name;
        return this;
    }

    /// <summary>
    /// Sets the query date (appointment date)
    /// </summary>
    public MaterializedViewsTestDataBuilder WithQueryDate(DateOnly date)
    {
        _queryDate = date;
        return this;
    }

    /// <summary>
    /// Sets the time slot details
    /// </summary>
    public MaterializedViewsTestDataBuilder WithTimeSlot(int sequenceOrder, TimeOnly startTime, TimeOnly endTime)
    {
        _sequenceOrder = sequenceOrder;
        _slotStartTime = startTime;
        _slotEndTime = endTime;
        return this;
    }

    /// <summary>
    /// Sets availability status
    /// </summary>
    public MaterializedViewsTestDataBuilder WithAvailability(bool isAvailable)
    {
        _isAvailable = isAvailable;
        return this;
    }

    /// <summary>
    /// Creates a complete test scenario with dealership, technician, and service type
    /// </summary>
    public MaterializedViewsScenario BuildCompleteScenario()
    {
        var dealership = DealershipBuilder.ValidDealership()
            .WithId(_dealershipId)
            .Build();

        var technician = TechnicianBuilder.ValidTechnician()
            .WithId(_technicianId)
            .WithDealershipId(_dealershipId)
            .WithFirstName(_technicianFirstName)
            .WithLastName(_technicianLastName)
            .Build();

        var serviceType = ServiceTypeBuilder.ValidServiceType()
            .WithId(_serviceTypeId)
            .WithName(_serviceTypeName)
            .WithDurationMinutes(_durationMinutes)
            .Build();

        var serviceBay = ServiceBayBuilder.ValidServiceBay()
            .WithId(_serviceBayId)
            .WithName(_serviceBayName)
            .WithDealershipId(_dealershipId)
            .Build();

        return new MaterializedViewsScenario
        {
            Dealership = dealership,
            Technician = technician,
            ServiceType = serviceType,
            ServiceBay = serviceBay,
            QueryDate = _queryDate,
            DurationMinutes = _durationMinutes,
            RequiredSlots = CalculateRequiredSlots(_durationMinutes)
        };
    }

    /// <summary>
    /// Builds a TechnicianAvailableSlot DTO
    /// </summary>
    public TechnicianAvailableSlotsView BuildTechnicianAvailableSlot()
    {
        return new TechnicianAvailableSlotsView
        {
            TimeSlotId = Guid.NewGuid(),
            SequenceOrder = _sequenceOrder,
            SlotStartTime = _slotStartTime,
            SlotEndTime = _slotEndTime,
            TechnicianId = _technicianId,
            FirstName = _technicianFirstName,
            LastName = _technicianLastName,
            DealershipId = _dealershipId,
            QueryDate = _queryDate,
            IsAvailable = _isAvailable
        };
    }

    /// <summary>
    /// Builds a collection of TechnicianAvailableSlot DTOs (all 18 slots of the day)
    /// </summary>
    public List<TechnicianAvailableSlotsView> BuildTechnicianAvailableSlotsForDay()
    {
        var slots = new List<TechnicianAvailableSlotsView>();
        var currentTime = new TimeOnly(8, 0); // 08:00
        
        for (int i = 1; i <= 18; i++)
        {
            var endTime = currentTime.AddMinutes(30);
            
            slots.Add(new TechnicianAvailableSlotsView
            {
                TimeSlotId = Guid.NewGuid(),
                SequenceOrder = i,
                SlotStartTime = currentTime,
                SlotEndTime = endTime,
                TechnicianId = _technicianId,
                FirstName = _technicianFirstName,
                LastName = _technicianLastName,
                DealershipId = _dealershipId,
                QueryDate = _queryDate,
                IsAvailable = _isAvailable
            });

            currentTime = endTime;
        }

        return slots;
    }

    /// <summary>
    /// Builds a ServiceBayAvailableSlot DTO
    /// </summary>
    public ServiceBayAvailableSlotsView BuildServiceBayAvailableSlot()
    {
        return new ServiceBayAvailableSlotsView
        {
            TimeSlotId = Guid.NewGuid(),
            SequenceOrder = _sequenceOrder,
            SlotStartTime = _slotStartTime,
            SlotEndTime = _slotEndTime,
            ServiceBayId = _serviceBayId,
            ServiceBayName = _serviceBayName,
            DealershipId = _dealershipId,
            QueryDate = _queryDate,
            IsAvailable = _isAvailable
        };
    }

    /// <summary>
    /// Builds a collection of ServiceBayAvailableSlot DTOs (all 18 slots of the day)
    /// </summary>
    public List<ServiceBayAvailableSlotsView> BuildServiceBayAvailableSlotsForDay()
    {
        var slots = new List<ServiceBayAvailableSlotsView>();
        var currentTime = new TimeOnly(8, 0); // 08:00

        for (int i = 1; i <= 18; i++)
        {
            var endTime = currentTime.AddMinutes(30);

            slots.Add(new ServiceBayAvailableSlotsView
            {
                TimeSlotId = Guid.NewGuid(),
                SequenceOrder = i,
                SlotStartTime = currentTime,
                SlotEndTime = endTime,
                ServiceBayId = _serviceBayId,
                ServiceBayName = _serviceBayName,
                DealershipId = _dealershipId,
                QueryDate = _queryDate,
                IsAvailable = _isAvailable
            });

            currentTime = endTime;
        }

        return slots;
    }

    /// <summary>
    /// Builds a ServiceTypeAvailabilityView DTO
    /// </summary>
    public ServiceTypeAvailabilityView BuildServiceTypeAvailability()
    {
        return new ServiceTypeAvailabilityView
        {
            ServiceTypeId = _serviceTypeId,
            ServiceTypeName = _serviceTypeName,
            DurationMinutes = _durationMinutes,
            RequiredSlots = CalculateRequiredSlots(_durationMinutes),
            TimeSlotId = Guid.NewGuid(),
            SequenceOrder = _sequenceOrder,
            SlotStartTime = _slotStartTime,
            SlotEndTime = _slotEndTime,
            TechnicianId = _technicianId,
            FirstName = _technicianFirstName,
            LastName = _technicianLastName,
            ServiceBayId = _serviceBayId,
            ServiceBayName = _serviceBayName,
            DealershipId = _dealershipId,
            QueryDate = _queryDate,
            CanFitService = _isAvailable
        };
    }

    /// <summary>
    /// Builds a collection of ServiceTypeAvailabilityView DTOs with multiple technicians and bays
    /// </summary>
    public List<ServiceTypeAvailabilityView> BuildServiceTypeAvailabilityOptions(
        int technicianCount = 2,
        int bayCount = 2,
        int slotsToCheck = 3)
    {
        var options = new List<ServiceTypeAvailabilityView>();
        var currentTime = new TimeOnly(8, 0);

        for (int slot = 1; slot <= slotsToCheck; slot++)
        {
            for (int tech = 1; tech <= technicianCount; tech++)
            {
                for (int bay = 1; bay <= bayCount; bay++)
                {
                    options.Add(new ServiceTypeAvailabilityView
                    {
                        ServiceTypeId = _serviceTypeId,
                        ServiceTypeName = _serviceTypeName,
                        DurationMinutes = _durationMinutes,
                        RequiredSlots = CalculateRequiredSlots(_durationMinutes),
                        TimeSlotId = Guid.NewGuid(),
                        SequenceOrder = slot,
                        SlotStartTime = currentTime,
                        SlotEndTime = currentTime.AddMinutes(30),
                        TechnicianId = Guid.NewGuid(),
                        FirstName = $"Tech{tech}",
                        LastName = $"Last{tech}",
                        ServiceBayId = Guid.NewGuid(),
                        ServiceBayName = $"Bay {bay}",
                        DealershipId = _dealershipId,
                        QueryDate = _queryDate,
                        CanFitService = _isAvailable
                    });
                }
            }

            currentTime = currentTime.AddMinutes(30);
        }

        return options;
    }

    /// <summary>
    /// Builds a Service entity with TimeSlot references for conflict testing
    /// </summary>
    public Service BuildServiceWithTimeSlots(
        Guid appointmentId,
        Guid startTimeSlotId,
        Guid endTimeSlotId,
        Guid appointmentDateAsSlot = default)
    {
        return new Service
        {
            Id = Guid.NewGuid(),
            AppointmentId = appointmentId,
            ServiceTypeId = _serviceTypeId,
            TechnicianId = _technicianId,
            ServiceBayId = _serviceBayId,
            DealershipId = _dealershipId,
            ServiceStatusId = Guid.Parse("00000000-0000-0000-0001-000000000001"), // Pending
            EstimatedStartTimeSlotId = startTimeSlotId,
            EstimatedEndTimeSlotId = endTimeSlotId,
            SequenceOrder = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Builds an Appointment entity for conflict testing
    /// </summary>
    public Appointment BuildAppointment(DateOnly? appointmentDate = null)
    {
        return new Appointment
        {
            Id = Guid.NewGuid(),
            DealershipId = _dealershipId,
            AppointmentDate = appointmentDate ?? _queryDate,
            StatusId = Guid.Parse("00000000-0000-0000-0000-000000000001"), // Booked
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Helper: Calculates required slots from duration in minutes
    /// </summary>
    private static int CalculateRequiredSlots(int durationMinutes)
    {
        return (int)Math.Ceiling(durationMinutes / 30.0);
    }
}

/// <summary>
/// Test scenario container for materialized views tests
/// Contains all related entities needed for a complete test
/// </summary>
public class MaterializedViewsScenario
{
    /// <summary>
    /// The dealership (root entity)
    /// </summary>
    public Dealership Dealership { get; set; } = null!;

    /// <summary>
    /// A technician in the dealership
    /// </summary>
    public Technician Technician { get; set; } = null!;

    /// <summary>
    /// A service type available at the dealership
    /// </summary>
    public ServiceType ServiceType { get; set; } = null!;

    /// <summary>
    /// A service bay at the dealership
    /// </summary>
    public ServiceBay ServiceBay { get; set; } = null!;

    /// <summary>
    /// The query date (appointment date for view queries)
    /// </summary>
    public DateOnly QueryDate { get; set; }

    /// <summary>
    /// Service duration in minutes
    /// </summary>
    public int DurationMinutes { get; set; }

    /// <summary>
    /// Number of 30-min slots required for this service
    /// </summary>
    public int RequiredSlots { get; set; }
}

/// <summary>
/// Builder for TimeSlot entities used in view testing
/// </summary>
public class TimeSlotTestBuilder
{
    private Guid _id = Guid.NewGuid();
    private int _sequenceOrder = 1;
    private TimeOnly _slotStartTime = new TimeOnly(8, 0);
    private TimeOnly _slotEndTime = new TimeOnly(8, 30);
    private bool _isActive = true;

    public static TimeSlotTestBuilder CreateValid()
    {
        return new TimeSlotTestBuilder();
    }

    public TimeSlotTestBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public TimeSlotTestBuilder WithSequenceOrder(int sequenceOrder)
    {
        _sequenceOrder = sequenceOrder;
        _slotStartTime = new TimeOnly(8, 0).AddMinutes((sequenceOrder - 1) * 30);
        _slotEndTime = _slotStartTime.AddMinutes(30);
        return this;
    }

    public TimeSlotTestBuilder WithTimeRange(TimeOnly start, TimeOnly end)
    {
        _slotStartTime = start;
        _slotEndTime = end;
        return this;
    }

    public TimeSlotTestBuilder WithActive(bool isActive)
    {
        _isActive = isActive;
        return this;
    }

    public TimeSlot Build()
    {
        return new TimeSlot
        {
            Id = _id,
            SequenceOrder = _sequenceOrder,
            SlotStartTime = _slotStartTime,
            SlotEndTime = _slotEndTime,
            IsActive = _isActive,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Builds all 18 time slots for a business day (08:00-17:00)
    /// </summary>
    public static List<TimeSlot> BuildAllDaySlots()
    {
        var slots = new List<TimeSlot>();
        var currentTime = new TimeOnly(8, 0);

        for (int i = 1; i <= 18; i++)
        {
            var endTime = currentTime.AddMinutes(30);

            slots.Add(new TimeSlot
            {
                Id = Guid.NewGuid(),
                SequenceOrder = i,
                SlotStartTime = currentTime,
                SlotEndTime = endTime,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

            currentTime = endTime;
        }

        return slots;
    }
}

/// <summary>
/// Builder for Service entity with view-related configuration
/// </summary>
public class ServiceForViewTestBuilder
{
    private Guid _id = Guid.NewGuid();
    private Guid _appointmentId = Guid.NewGuid();
    private Guid _serviceTypeId = Guid.NewGuid();
    private Guid? _technicianId = Guid.NewGuid();
    private Guid? _serviceBayId = Guid.NewGuid();
    private Guid _dealershipId = Guid.NewGuid();
    private Guid _serviceStatusId = Guid.Parse("00000000-0000-0000-0001-000000000001"); // Pending
    private Guid? _estimatedStartTimeSlotId = Guid.NewGuid();
    private Guid? _estimatedEndTimeSlotId = Guid.NewGuid();
    private int _sequenceOrder = 1;

    public static ServiceForViewTestBuilder CreateValid()
    {
        return new ServiceForViewTestBuilder();
    }

    public ServiceForViewTestBuilder WithAppointmentId(Guid appointmentId)
    {
        _appointmentId = appointmentId;
        return this;
    }

    public ServiceForViewTestBuilder WithServiceTypeId(Guid serviceTypeId)
    {
        _serviceTypeId = serviceTypeId;
        return this;
    }

    public ServiceForViewTestBuilder WithTechnicianId(Guid? technicianId)
    {
        _technicianId = technicianId;
        return this;
    }

    public ServiceForViewTestBuilder WithServiceBayId(Guid? serviceBayId)
    {
        _serviceBayId = serviceBayId;
        return this;
    }

    public ServiceForViewTestBuilder WithDealershipId(Guid dealershipId)
    {
        _dealershipId = dealershipId;
        return this;
    }

    public ServiceForViewTestBuilder WithTimeSlots(Guid startSlotId, Guid endSlotId)
    {
        _estimatedStartTimeSlotId = startSlotId;
        _estimatedEndTimeSlotId = endSlotId;
        return this;
    }

    public ServiceForViewTestBuilder WithoutTechnicianAssignment()
    {
        _technicianId = null;
        return this;
    }

    public ServiceForViewTestBuilder WithoutBayAssignment()
    {
        _serviceBayId = null;
        return this;
    }

    public Service Build()
    {
        return new Service
        {
            Id = _id,
            AppointmentId = _appointmentId,
            ServiceTypeId = _serviceTypeId,
            TechnicianId = _technicianId,
            ServiceBayId = _serviceBayId,
            DealershipId = _dealershipId,
            ServiceStatusId = _serviceStatusId,
            EstimatedStartTimeSlotId = _estimatedStartTimeSlotId,
            EstimatedEndTimeSlotId = _estimatedEndTimeSlotId,
            SequenceOrder = _sequenceOrder,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}
