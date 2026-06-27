using VehicleServiceBooking.Application.Models.ViewModels;
using VehicleServiceBooking.Domain.Entities;

namespace VehicleServiceBooking.Tests.Common;

/// <summary>
/// Test data builder for Dealership entity.
/// Provides a fluent API to create test dealerships with customizable values.
/// </summary>
public class DealershipBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _name = "Test Dealership";
    private string _address = "123 Main Street";

    public static DealershipBuilder ValidDealership()
    {
        return new DealershipBuilder();
    }

    public DealershipBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public DealershipBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public DealershipBuilder WithAddress(string address)
    {
        _address = address;
        return this;
    }

    public Dealership Build()
    {
        return new Dealership
        {
            Id = _id,
            Name = _name,
            Address = _address
        };
    }
}

/// <summary>
/// Test data builder for ServiceType entity.
/// Provides a fluent API to create test service types with customizable values.
/// </summary>
public class ServiceTypeBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _name = "Oil Change";
    private int _durationMinutes = 30;

    public static ServiceTypeBuilder ValidServiceType()
    {
        return new ServiceTypeBuilder();
    }

    public ServiceTypeBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public ServiceTypeBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public ServiceTypeBuilder WithDurationMinutes(int durationMinutes)
    {
        _durationMinutes = durationMinutes;
        return this;
    }

    public ServiceTypeBuilder QuickService()
    {
        _name = "Quick Oil Change";
        _durationMinutes = 15;
        return this;
    }

    public ServiceTypeBuilder StandardService()
    {
        _name = "Standard Service";
        _durationMinutes = 30;
        return this;
    }

    public ServiceTypeBuilder MajorService()
    {
        _name = "Major Service";
        _durationMinutes = 90;
        return this;
    }

    public ServiceType Build()
    {
        return new ServiceType
        {
            Id = _id,
            Name = _name,
            DurationMinutes = _durationMinutes
        };
    }
}

/// <summary>
/// Test data builder for Technician entity.
/// Provides a fluent API to create test technicians with customizable values.
/// </summary>
public class TechnicianBuilder
{
    private Guid _id = Guid.NewGuid();
    private Guid _dealershipId = Guid.NewGuid();
    private string _firstName = "John";
    private string _lastName = "Doe";

    public static TechnicianBuilder ValidTechnician()
    {
        return new TechnicianBuilder();
    }

    public TechnicianBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public TechnicianBuilder WithDealershipId(Guid dealershipId)
    {
        _dealershipId = dealershipId;
        return this;
    }

    public TechnicianBuilder WithFirstName(string firstName)
    {
        _firstName = firstName;
        return this;
    }

    public TechnicianBuilder WithLastName(string lastName)
    {
        _lastName = lastName;
        return this;
    }

    public Technician Build()
    {
        return new Technician
        {
            Id = _id,
            DealershipId = _dealershipId,
            FirstName = _firstName,
            LastName = _lastName
        };
    }
}

/// <summary>
/// Test data builder for TechnicianSkill entity.
/// Provides a fluent API to create test technician skills with customizable values.
/// </summary>
public class TechnicianSkillBuilder
{
    private Guid _technicianId = Guid.NewGuid();
    private Guid _serviceTypeId = Guid.NewGuid();

    public static TechnicianSkillBuilder ValidSkill()
    {
        return new TechnicianSkillBuilder();
    }

    public TechnicianSkillBuilder WithTechnicianId(Guid technicianId)
    {
        _technicianId = technicianId;
        return this;
    }

    public TechnicianSkillBuilder WithServiceTypeId(Guid serviceTypeId)
    {
        _serviceTypeId = serviceTypeId;
        return this;
    }

    public TechnicianSkill Build()
    {
        return new TechnicianSkill
        {
            TechnicianId = _technicianId,
            ServiceTypeId = _serviceTypeId
        };
    }
}

/// <summary>
/// Test data builder for TechnicianSchedule entity.
/// Provides a fluent API to create test technician schedules with customizable values.
/// </summary>
public class TechnicianScheduleBuilder
{
    private Guid _id = Guid.NewGuid();
    private Guid _technicianId = Guid.NewGuid();
    private DayOfWeek _dayOfWeek = DateTime.UtcNow.DayOfWeek;
    private TimeOnly _startTime = new TimeOnly(8, 0);
    private TimeOnly _endTime = new TimeOnly(17, 0);

    public static TechnicianScheduleBuilder ValidSchedule()
    {
        return new TechnicianScheduleBuilder();
    }

    public TechnicianScheduleBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public TechnicianScheduleBuilder WithTechnicianId(Guid technicianId)
    {
        _technicianId = technicianId;
        return this;
    }

    public TechnicianScheduleBuilder WithDayOfWeek(DayOfWeek dayOfWeek)
    {
        _dayOfWeek = dayOfWeek;
        return this;
    }

    public TechnicianScheduleBuilder WithStartTime(TimeOnly startTime)
    {
        _startTime = startTime;
        return this;
    }

    public TechnicianScheduleBuilder WithEndTime(TimeOnly endTime)
    {
        _endTime = endTime;
        return this;
    }

    public TechnicianScheduleBuilder WithWorkingHours(TimeOnly start, TimeOnly end)
    {
        _startTime = start;
        _endTime = end;
        return this;
    }

    public TechnicianScheduleBuilder MorningShift()
    {
        _startTime = new TimeOnly(6, 0);
        _endTime = new TimeOnly(14, 0);
        return this;
    }

    public TechnicianScheduleBuilder AfternoonShift()
    {
        _startTime = new TimeOnly(14, 0);
        _endTime = new TimeOnly(22, 0);
        return this;
    }

    public TechnicianScheduleBuilder StandardHours()
    {
        _startTime = new TimeOnly(8, 0);
        _endTime = new TimeOnly(17, 0);
        return this;
    }

    public TechnicianScheduleBuilder RestrictedHours()
    {
        _startTime = new TimeOnly(10, 0);
        _endTime = new TimeOnly(14, 0);
        return this;
    }

    public TechnicianSchedule Build()
    {
        return new TechnicianSchedule
        {
            Id = _id,
            TechnicianId = _technicianId,
            DayOfWeek = _dayOfWeek,
            StartTime = _startTime,
            EndTime = _endTime
        };
    }
}

/// <summary>
/// Test data builder for Customer entity.
/// Provides a fluent API to create test customers with customizable values.
/// </summary>
public class CustomerBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _firstName = "John";
    private string _lastName = "Doe";
    private string _email = "john.doe@example.com";
    private string _phoneNumber = "555-1234";

    public static CustomerBuilder ValidCustomer()
    {
        return new CustomerBuilder();
    }

    public CustomerBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public CustomerBuilder WithFirstName(string firstName)
    {
        _firstName = firstName;
        return this;
    }

    public CustomerBuilder WithLastName(string lastName)
    {
        _lastName = lastName;
        return this;
    }

    public CustomerBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public CustomerBuilder WithPhoneNumber(string phoneNumber)
    {
        _phoneNumber = phoneNumber;
        return this;
    }

    public Customer Build()
    {
        return new Customer
        {
            Id = _id,
            FirstName = _firstName,
            LastName = _lastName,
            Email = _email,
            PhoneNumber = _phoneNumber
        };
    }
}

/// <summary>
/// Test data builder for Vehicle entity.
/// Provides a fluent API to create test vehicles with customizable values.
/// </summary>
public class VehicleBuilder
{
    private Guid _id = Guid.NewGuid();
    private Guid _customerId = Guid.NewGuid();
    private string _vin = "1HGCV1F32LB123456";
    private string _make = "Honda";
    private string _model = "Civic";
    private int? _year = 2020;

    public static VehicleBuilder ValidVehicle()
    {
        return new VehicleBuilder();
    }

    public VehicleBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public VehicleBuilder WithCustomerId(Guid customerId)
    {
        _customerId = customerId;
        return this;
    }

    public VehicleBuilder WithVin(string vin)
    {
        _vin = vin;
        return this;
    }

    public VehicleBuilder WithMake(string make)
    {
        _make = make;
        return this;
    }

    public VehicleBuilder WithModel(string model)
    {
        _model = model;
        return this;
    }

    public VehicleBuilder WithYear(int? year)
    {
        _year = year;
        return this;
    }

    public Vehicle Build()
    {
        return new Vehicle
        {
            Id = _id,
            CustomerId = _customerId,
            Vin = _vin,
            Make = _make,
            Model = _model,
            Year = _year
        };
    }
}

/// <summary>
/// Test data builder for ServiceBay entity.
/// Provides a fluent API to create test service bays with customizable values.
/// </summary>
public class ServiceBayBuilder
{
    private Guid _id = Guid.NewGuid();
    private Guid _dealershipId = Guid.NewGuid();
    private string _name = "Bay 1";

    public static ServiceBayBuilder ValidServiceBay()
    {
        return new ServiceBayBuilder();
    }

    public ServiceBayBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public ServiceBayBuilder WithDealershipId(Guid dealershipId)
    {
        _dealershipId = dealershipId;
        return this;
    }

    public ServiceBayBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public ServiceBay Build()
    {
        return new ServiceBay
        {
            Id = _id,
            DealershipId = _dealershipId,
            Name = _name
        };
    }
}

/// <summary>
/// Test data builder for Appointment entity.
/// Provides a fluent API to create test appointments with customizable values.
/// </summary>
public class AppointmentBuilder
{
    private Guid _id = Guid.NewGuid();
    private Guid _dealershipId = Guid.NewGuid();
    private Guid _technicianId = Guid.NewGuid();
    private Guid _serviceBayId = Guid.NewGuid();
    private DateTime _startTime = DateTime.UtcNow.AddHours(1);
    private DateTime _endTime = DateTime.UtcNow.AddHours(2);
    private Guid _statusId = Guid.Parse("00000000-0000-0000-0000-000000000001"); // Booked

    public static AppointmentBuilder ValidAppointment()
    {
        return new AppointmentBuilder();
    }

    public AppointmentBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public AppointmentBuilder WithDealershipId(Guid dealershipId)
    {
        _dealershipId = dealershipId;
        return this;
    }

    public AppointmentBuilder WithTechnicianId(Guid technicianId)
    {
        _technicianId = technicianId;
        return this;
    }

    public AppointmentBuilder WithServiceBayId(Guid serviceBayId)
    {
        _serviceBayId = serviceBayId;
        return this;
    }

    public AppointmentBuilder WithStartTime(DateTime startTime)
    {
        _startTime = startTime;
        _endTime = startTime.AddHours(1);
        return this;
    }

    public AppointmentBuilder WithTimeRange(DateTime startTime, DateTime endTime)
    {
        _startTime = startTime;
        _endTime = endTime;
        return this;
    }

    public AppointmentBuilder WithStatus(Domain.Enums.AppointmentStatus status)
    {
        _statusId = status switch
        {
            Domain.Enums.AppointmentStatus.Booked => Guid.Parse("00000000-0000-0000-0000-000000000001"),
            Domain.Enums.AppointmentStatus.InProgress => Guid.Parse("00000000-0000-0000-0000-000000000002"),
            Domain.Enums.AppointmentStatus.Completed => Guid.Parse("00000000-0000-0000-0000-000000000003"),
            Domain.Enums.AppointmentStatus.Cancelled => Guid.Parse("00000000-0000-0000-0000-000000000004"),
            _ => Guid.Parse("00000000-0000-0000-0000-000000000001")
        };
        return this;
    }

    public AppointmentBuilder InProgress()
    {
        _statusId = Guid.Parse("00000000-0000-0000-0000-000000000002");
        return this;
    }

    public AppointmentBuilder Booked()
    {
        _statusId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        return this;
    }

    public AppointmentBuilder Cancelled()
    {
        _statusId = Guid.Parse("00000000-0000-0000-0000-000000000004");
        return this;
    }

    public Appointment Build()
    {
        return new Appointment
        {
            Id = _id,
            DealershipId = _dealershipId,
            AppointmentDate = DateOnly.FromDateTime(_startTime),
            StatusId = _statusId
        };
    }
}

/// <summary>
/// Composite builder for availability test scenarios.
/// Creates complete test setups with all required entities.
/// </summary>
public class AvailabilityTestScenarioBuilder
{
    private Guid _dealershipId = Guid.NewGuid();
    private Guid _serviceTypeId = Guid.NewGuid();
    private Guid _technicianId = Guid.NewGuid();
    private Guid _serviceBayId = Guid.NewGuid();
    private DateTime _testDate = DateTime.UtcNow.AddDays(1).Date.AddHours(9);
    private TimeOnly _technicianStartTime = new TimeOnly(8, 0);
    private TimeOnly _technicianEndTime = new TimeOnly(17, 0);
    private int _serviceDurationMinutes = 30;
    private string _serviceName = "Oil Change";
    private bool _includeExistingAppointment = false;
    private DateTime? _existingAppointmentStart;
    private DateTime? _existingAppointmentEnd;
    private int _numberOfTechnicians = 1;
    private int _numberOfServiceBays = 1;
    private List<Appointment> _existingAppointments = new();

    public static AvailabilityTestScenarioBuilder SimpleScenario()
    {
        return new AvailabilityTestScenarioBuilder();
    }

    public AvailabilityTestScenarioBuilder WithDealershipId(Guid dealershipId)
    {
        _dealershipId = dealershipId;
        return this;
    }

    public AvailabilityTestScenarioBuilder WithServiceTypeId(Guid serviceTypeId)
    {
        _serviceTypeId = serviceTypeId;
        return this;
    }

    public AvailabilityTestScenarioBuilder WithTechnicianId(Guid technicianId)
    {
        _technicianId = technicianId;
        return this;
    }

    public AvailabilityTestScenarioBuilder WithServiceBayId(Guid serviceBayId)
    {
        _serviceBayId = serviceBayId;
        return this;
    }

    public AvailabilityTestScenarioBuilder WithTestDate(DateTime date)
    {
        _testDate = date;
        return this;
    }

    public AvailabilityTestScenarioBuilder WithTechnicianSchedule(TimeOnly startTime, TimeOnly endTime)
    {
        _technicianStartTime = startTime;
        _technicianEndTime = endTime;
        return this;
    }

    public AvailabilityTestScenarioBuilder WithServiceDuration(int durationMinutes)
    {
        _serviceDurationMinutes = durationMinutes;
        return this;
    }

    public AvailabilityTestScenarioBuilder WithNumberOfTechnicians(int count)
    {
        _numberOfTechnicians = count;
        return this;
    }

    public AvailabilityTestScenarioBuilder WithNumberOfServiceBays(int count)
    {
        _numberOfServiceBays = count;
        return this;
    }

    public AvailabilityTestScenarioBuilder WithServiceName(string serviceName)
    {
        _serviceName = serviceName;
        return this;
    }

    public AvailabilityTestScenarioBuilder WithExistingAppointment(Appointment appointment)
    {
        _includeExistingAppointment = true;
        _existingAppointments.Add(appointment);
        _existingAppointmentStart = appointment.AppointmentDate.ToDateTime(new TimeOnly(8, 0));
        _existingAppointmentEnd = appointment.AppointmentDate.ToDateTime(new TimeOnly(8, 30));
        return this;
    }

    public AvailabilityTestScenarioBuilder WithExistingAppointments(params Appointment[] appointments)
    {
        _includeExistingAppointment = true;
        _existingAppointments.AddRange(appointments);
        if (appointments.Length > 0)
        {
            _existingAppointmentStart = appointments[0].AppointmentDate.ToDateTime(new TimeOnly(8, 0));
            _existingAppointmentEnd = appointments[0].AppointmentDate.ToDateTime(new TimeOnly(8, 30));
        }
        return this;
    }

    public (Dealership, ServiceType, List<Technician>, List<TechnicianSkill>, 
             List<TechnicianSchedule>, List<ServiceBay>, List<Appointment>, List<ServiceTypeAvailabilityView>) Build()
    {
        var dealership = new DealershipBuilder()
            .WithId(_dealershipId)
            .Build();

        var serviceType = new ServiceTypeBuilder()
            .WithId(_serviceTypeId)
            .WithDurationMinutes(_serviceDurationMinutes)
            .Build();

        var technicians = new List<Technician>();
        var technicianSkills = new List<TechnicianSkill>();
        var technicianSchedules = new List<TechnicianSchedule>();

        for (int i = 0; i < _numberOfTechnicians; i++)
        {
            var techId = i == 0 ? _technicianId : Guid.NewGuid();
            
            technicians.Add(new TechnicianBuilder()
                .WithId(techId)
                .WithDealershipId(_dealershipId)
                .WithFirstName($"Tech{i + 1}")
                .WithLastName("Engineer")
                .Build());

            technicianSkills.Add(new TechnicianSkillBuilder()
                .WithTechnicianId(techId)
                .WithServiceTypeId(_serviceTypeId)
                .Build());

            technicianSchedules.Add(new TechnicianScheduleBuilder()
                .WithTechnicianId(techId)
                .WithDayOfWeek(_testDate.DayOfWeek)
                .WithWorkingHours(_technicianStartTime, _technicianEndTime)
                .Build());
        }

        var serviceBays = new List<ServiceBay>();
        for (int i = 0; i < _numberOfServiceBays; i++)
        {
            var bayId = i == 0 ? _serviceBayId : Guid.NewGuid();
            
            serviceBays.Add(new ServiceBayBuilder()
                .WithId(bayId)
                .WithDealershipId(_dealershipId)
                .WithName($"Bay {i + 1}")
                .Build());
        }

        var viewRows = BuildServiceTypeAvailabilityViews(technicians, serviceBays);

        return (dealership, serviceType, technicians, technicianSkills, technicianSchedules, serviceBays, _existingAppointments, viewRows);
    }

    private List<ServiceTypeAvailabilityView> BuildServiceTypeAvailabilityViews(
        List<Technician> technicians,
        List<ServiceBay> serviceBays)
    {
        var rows = new List<ServiceTypeAvailabilityView>();
        var availableStartTimes = new List<TimeOnly>();

        if (_includeExistingAppointment && _existingAppointmentStart.HasValue && _existingAppointmentEnd.HasValue)
        {
            var conflictStart = TimeOnly.FromDateTime(_existingAppointmentStart.Value);
            var conflictEnd = TimeOnly.FromDateTime(_existingAppointmentEnd.Value);

            // Build morning slots around the existing appointment
            availableStartTimes.Add(new TimeOnly(8, 0));
            if (conflictStart > new TimeOnly(8, 0))
            {
                availableStartTimes.Add(conflictStart.AddMinutes(-30));
            }
            availableStartTimes.Add(conflictEnd);
        }
        else
        {
            availableStartTimes.Add(new TimeOnly(8, 0));
            availableStartTimes.Add(new TimeOnly(8, 30));
            availableStartTimes.Add(new TimeOnly(9, 0));
        }

        if (_technicianStartTime > availableStartTimes[0])
        {
            availableStartTimes = availableStartTimes
                .Where(start => start >= _technicianStartTime)
                .ToList();
        }

        foreach (var startTime in availableStartTimes)
        {
            var endTime = startTime.AddMinutes(_serviceDurationMinutes);
            if (endTime > _technicianEndTime)
            {
                continue;
            }
            foreach (var technician in technicians)
            {
                foreach (var bay in serviceBays)
                {
                    var canFit = true;
                    if (_includeExistingAppointment && _existingAppointmentStart.HasValue && _existingAppointmentEnd.HasValue)
                    {
                        var conflictStart = TimeOnly.FromDateTime(_existingAppointmentStart.Value);
                        var conflictEnd = TimeOnly.FromDateTime(_existingAppointmentEnd.Value);
                        if (startTime < conflictEnd && endTime > conflictStart && bay.Id == _serviceBayId)
                        {
                            canFit = false;
                        }
                    }

                    rows.Add(new ServiceTypeAvailabilityView
                    {
                        ServiceTypeId = _serviceTypeId,
                        ServiceTypeName = _serviceName,
                        DurationMinutes = _serviceDurationMinutes,
                        RequiredSlots = _serviceDurationMinutes / 30,
                        TimeSlotId = Guid.NewGuid(),
                        SequenceOrder = GetSequenceOrder(startTime),
                        SlotStartTime = startTime,
                        SlotEndTime = endTime,
                        TechnicianId = technician.Id,
                        FirstName = technician.FirstName,
                        LastName = technician.LastName,
                        ServiceBayId = bay.Id,
                        ServiceBayName = bay.Name,
                        DealershipId = _dealershipId,
                        QueryDate = DateOnly.FromDateTime(_testDate),
                        CanFitService = canFit
                    });
                }
            }
        }

        if (_technicianStartTime > new TimeOnly(8, 0))
        {
            rows = rows.Where(row => row.SlotStartTime >= _technicianStartTime).ToList();
        }

        return rows;
    }

    private static int GetSequenceOrder(TimeOnly startTime)
    {
        var minutesSinceEight = ((startTime.Hour - 8) * 60) + startTime.Minute;
        return (minutesSinceEight / 30) + 1;
    }
}

/// <summary>
/// Integration test scenario builders for AvailabilityService
/// These builders prepare complete, database-ready test scenarios for integration tests
/// </summary>
public class AvailabilityIntegrationScenarioBuilder
{
    private readonly DateTime _date = DateTime.UtcNow.AddDays(1).Date.AddHours(9);
    private Guid _dealershipId = Guid.NewGuid();
    private Guid _serviceTypeId = Guid.NewGuid();
    private Guid _technicianId = Guid.NewGuid();
    private Guid _serviceBayId = Guid.NewGuid();
    private string _technicianFirstName = "John";
    private string _technicianLastName = "Doe";
    private string _serviceName = "Oil Change";
    private int _serviceDurationMinutes = 30;
    private TimeOnly _technicianStartTime = new TimeOnly(8, 0);
    private TimeOnly _technicianEndTime = new TimeOnly(17, 0);
    private string _serviceBayName = "Bay 1";
    private bool _includeExistingAppointment = false;
    private DateTime? _existingAppointmentStart;
    private DateTime? _existingAppointmentEnd;
    private int _numberOfTechnicians = 1;
    private int _numberOfBays = 1;

    public static AvailabilityIntegrationScenarioBuilder CompleteSetup()
    {
        return new AvailabilityIntegrationScenarioBuilder();
    }

    public static AvailabilityIntegrationScenarioBuilder WithExistingAppointmentConflict()
    {
        return new AvailabilityIntegrationScenarioBuilder()
            .WithExistingAppointment(true)
            .WithExistingAppointmentTime(DateTime.UtcNow.AddDays(1).Date.AddHours(9), 
                                        DateTime.UtcNow.AddDays(1).Date.AddHours(9).AddMinutes(30));
    }

    public static AvailabilityIntegrationScenarioBuilder MultipleTechnicians()
    {
        return new AvailabilityIntegrationScenarioBuilder()
            .WithNumberOfTechnicians(2);
    }

    public static AvailabilityIntegrationScenarioBuilder MultipleServiceBays()
    {
        return new AvailabilityIntegrationScenarioBuilder()
            .WithNumberOfBays(2);
    }

    public static AvailabilityIntegrationScenarioBuilder LongerServiceDuration()
    {
        return new AvailabilityIntegrationScenarioBuilder()
            .WithServiceDuration(90);
    }

    public static AvailabilityIntegrationScenarioBuilder RestrictedTechnicianSchedule()
    {
        return new AvailabilityIntegrationScenarioBuilder()
            .WithTechnicianWorkingHours(new TimeOnly(10, 0), new TimeOnly(14, 0));
    }

    public AvailabilityIntegrationScenarioBuilder WithDate(DateTime date)
    {
        return this;
    }

    public AvailabilityIntegrationScenarioBuilder WithDealershipId(Guid dealershipId)
    {
        _dealershipId = dealershipId;
        return this;
    }

    public AvailabilityIntegrationScenarioBuilder WithServiceTypeId(Guid serviceTypeId)
    {
        _serviceTypeId = serviceTypeId;
        return this;
    }

    public AvailabilityIntegrationScenarioBuilder WithTechnicianId(Guid technicianId)
    {
        _technicianId = technicianId;
        return this;
    }

    public AvailabilityIntegrationScenarioBuilder WithServiceBayId(Guid serviceBayId)
    {
        _serviceBayId = serviceBayId;
        return this;
    }

    public AvailabilityIntegrationScenarioBuilder WithTechnicianName(string firstName, string lastName)
    {
        _technicianFirstName = firstName;
        _technicianLastName = lastName;
        return this;
    }

    public AvailabilityIntegrationScenarioBuilder WithServiceName(string serviceName)
    {
        _serviceName = serviceName;
        return this;
    }

    public AvailabilityIntegrationScenarioBuilder WithServiceDuration(int durationMinutes)
    {
        _serviceDurationMinutes = durationMinutes;
        return this;
    }

    public AvailabilityIntegrationScenarioBuilder WithTechnicianWorkingHours(TimeOnly startTime, TimeOnly endTime)
    {
        _technicianStartTime = startTime;
        _technicianEndTime = endTime;
        return this;
    }

    public AvailabilityIntegrationScenarioBuilder WithServiceBayName(string name)
    {
        _serviceBayName = name;
        return this;
    }

    public AvailabilityIntegrationScenarioBuilder WithExistingAppointment(bool include)
    {
        _includeExistingAppointment = include;
        return this;
    }

    public AvailabilityIntegrationScenarioBuilder WithExistingAppointmentTime(DateTime start, DateTime end)
    {
        _existingAppointmentStart = start;
        _existingAppointmentEnd = end;
        return this;
    }

    public AvailabilityIntegrationScenarioBuilder WithNumberOfTechnicians(int count)
    {
        _numberOfTechnicians = count;
        return this;
    }

    public AvailabilityIntegrationScenarioBuilder WithNumberOfBays(int count)
    {
        _numberOfBays = count;
        return this;
    }

    /// <summary>
    /// Builds a complete integration test scenario with all required entities
    /// Returns: (Dealership, ServiceType, List<Technician>, List<TechnicianSkill>, 
    ///          List<TechnicianSchedule>, List<ServiceBay>, List<Appointment>, List<ServiceTypeAvailabilityView>)
    /// </summary>
    public (Dealership, ServiceType, List<Technician>, List<TechnicianSkill>, 
            List<TechnicianSchedule>, List<ServiceBay>, List<Appointment>, List<ServiceTypeAvailabilityView>) Build()
    {
        // Create dealership
        var dealership = new Dealership
        {
            Id = _dealershipId,
            Name = "Test Dealership",
            Address = "123 Main St"
        };

        // Create service type
        var serviceType = new ServiceType
        {
            Id = _serviceTypeId,
            Name = _serviceName,
            DurationMinutes = _serviceDurationMinutes
        };

        // Create technicians, skills, and schedules
        var technicians = new List<Technician>();
        var technicianSkills = new List<TechnicianSkill>();
        var technicianSchedules = new List<TechnicianSchedule>();

        for (int i = 0; i < _numberOfTechnicians; i++)
        {
            var techId = i == 0 ? _technicianId : Guid.NewGuid();
            var technician = new Technician
            {
                Id = techId,
                DealershipId = _dealershipId,
                FirstName = i == 0 ? _technicianFirstName : $"{_technicianFirstName}{i}",
                LastName = i == 0 ? _technicianLastName : $"{_technicianLastName}{i}"
            };
            technicians.Add(technician);

            var technicianSkill = new TechnicianSkill
            {
                Id = Guid.NewGuid(),
                TechnicianId = techId,
                ServiceTypeId = _serviceTypeId
            };
            technicianSkills.Add(technicianSkill);

            var technicianSchedule = new TechnicianSchedule
            {
                Id = Guid.NewGuid(),
                TechnicianId = techId,
                DayOfWeek = _date.DayOfWeek,
                StartTime = _technicianStartTime,
                EndTime = _technicianEndTime
            };
            technicianSchedules.Add(technicianSchedule);
        }

        // Create service bays
        var serviceBays = new List<ServiceBay>();
        for (int i = 0; i < _numberOfBays; i++)
        {
            var bayId = i == 0 ? _serviceBayId : Guid.NewGuid();
            var serviceBay = new ServiceBay
            {
                Id = bayId,
                DealershipId = _dealershipId,
                Name = i == 0 ? _serviceBayName : $"{_serviceBayName} {i + 1}"
            };
            serviceBays.Add(serviceBay);
        }

        // Create existing appointments if needed
        var existingAppointments = new List<Appointment>();
        if (_includeExistingAppointment && _existingAppointmentStart.HasValue && _existingAppointmentEnd.HasValue)
        {
            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                DealershipId = _dealershipId,
                CustomerId = Guid.NewGuid(),
                VehicleId = Guid.NewGuid(),
                AppointmentDate = DateOnly.FromDateTime(_existingAppointmentStart.Value),
                StatusId = Guid.Parse("00000000-0000-0000-0000-000000000001") // Booked
            };
            existingAppointments.Add(appointment);
        }

        var viewRows = BuildServiceTypeAvailabilityViews(technicians, serviceBays);
        return (dealership, serviceType, technicians, technicianSkills, 
            technicianSchedules, serviceBays, existingAppointments, viewRows);
    }

    private List<ServiceTypeAvailabilityView> BuildServiceTypeAvailabilityViews(
        List<Technician> technicians,
        List<ServiceBay> serviceBays)
    {
        var rows = new List<ServiceTypeAvailabilityView>();
        var availableStartTimes = new List<TimeOnly>();

        var businessStart = new TimeOnly(8, 0);
        var businessEnd = new TimeOnly(17, 0);
        var currentStart = businessStart;
        while (currentStart.AddMinutes(_serviceDurationMinutes) <= businessEnd)
        {
            availableStartTimes.Add(currentStart);
            currentStart = currentStart.AddMinutes(30);
        }

        if (_technicianStartTime > availableStartTimes[0])
        {
            availableStartTimes = availableStartTimes
                .Where(start => start >= _technicianStartTime)
                .ToList();
        }

        foreach (var startTime in availableStartTimes)
        {
            var endTime = startTime.AddMinutes(_serviceDurationMinutes);
            if (endTime > _technicianEndTime)
            {
                continue;
            }
            foreach (var technician in technicians)
            {
                foreach (var bay in serviceBays)
                {
                    var canFit = true;
                    if (_includeExistingAppointment && _existingAppointmentStart.HasValue && _existingAppointmentEnd.HasValue)
                    {
                        var conflictStart = TimeOnly.FromDateTime(_existingAppointmentStart.Value);
                        var conflictEnd = TimeOnly.FromDateTime(_existingAppointmentEnd.Value);
                        if (startTime < conflictEnd && endTime > conflictStart && bay.Id == _serviceBayId)
                        {
                            canFit = false;
                        }
                    }

                    rows.Add(new ServiceTypeAvailabilityView
                    {
                        ServiceTypeId = _serviceTypeId,
                        ServiceTypeName = _serviceName,
                        DurationMinutes = _serviceDurationMinutes,
                        RequiredSlots = _serviceDurationMinutes / 30,
                        TimeSlotId = Guid.NewGuid(),
                        SequenceOrder = GetSequenceOrder(startTime),
                        SlotStartTime = startTime,
                        SlotEndTime = endTime,
                        TechnicianId = technician.Id,
                        FirstName = technician.FirstName,
                        LastName = technician.LastName,
                        ServiceBayId = bay.Id,
                        ServiceBayName = bay.Name,
                        DealershipId = _dealershipId,
                        QueryDate = DateOnly.FromDateTime(_date),
                        CanFitService = canFit
                    });
                }
            }
        }

        if (_technicianStartTime > new TimeOnly(8, 0))
        {
            rows = rows.Where(row => row.SlotStartTime >= _technicianStartTime).ToList();
        }

        return rows;
    }

    private static int GetSequenceOrder(TimeOnly startTime)
    {
        var minutesSinceEight = ((startTime.Hour - 8) * 60) + startTime.Minute;
        return (minutesSinceEight / 30) + 1;
    }
}
