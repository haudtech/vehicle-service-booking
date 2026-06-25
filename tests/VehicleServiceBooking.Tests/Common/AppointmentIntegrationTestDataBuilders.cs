using VehicleServiceBooking.Domain.Entities;
using VehicleServiceBooking.Domain.Enums;

namespace VehicleServiceBooking.Tests.Common;

/// <summary>
/// Integration test scenario builders for AppointmentService
/// These builders prepare complete, database-ready test scenarios for integration tests
/// Follows the same fluent builder pattern as other test data builders
/// </summary>
public class AppointmentIntegrationScenarioBuilder
{
    private Guid _dealershipId = Guid.NewGuid();
    private Guid _customerId = Guid.NewGuid();
    private Guid _vehicleId = Guid.NewGuid();
    private Guid _serviceTypeId = Guid.NewGuid();
    private Guid _technicianId = Guid.NewGuid();
    private Guid _serviceBayId = Guid.NewGuid();

    private string _dealershipName = "Test Dealership";
    private string _dealershipAddress = "123 Main St";
    private string _customerFirstName = "John";
    private string _customerLastName = "Customer";
    private string _customerEmail = "john@example.com";
    private string _vehicleMake = "Toyota";
    private string _vehicleModel = "Camry";
    private int _vehicleYear = 2024;
    private string _serviceName = "Oil Change";
    private int _serviceDurationMinutes = 30;
    private string _technicianFirstName = "John";
    private string _technicianLastName = "Technician";
    private string _serviceBayName = "Bay 1";

    private bool _includeExistingAppointments = false;
    private int _existingAppointmentCount = 1;
    private int _numberOfTechnicians = 1;
    private int _numberOfServiceBays = 1;

    public static AppointmentIntegrationScenarioBuilder CompleteSetup()
    {
        return new AppointmentIntegrationScenarioBuilder();
    }

    public static AppointmentIntegrationScenarioBuilder WithExistingAppointments()
    {
        return new AppointmentIntegrationScenarioBuilder()
            .IncludeExistingAppointments(true);
    }

    public static AppointmentIntegrationScenarioBuilder MultipleTechnicians()
    {
        return new AppointmentIntegrationScenarioBuilder()
            .WithNumberOfTechnicians(3);
    }

    public static AppointmentIntegrationScenarioBuilder MultipleServiceBays()
    {
        return new AppointmentIntegrationScenarioBuilder()
            .WithNumberOfServiceBays(3);
    }

    public AppointmentIntegrationScenarioBuilder WithDealershipId(Guid dealershipId)
    {
        _dealershipId = dealershipId;
        return this;
    }

    public AppointmentIntegrationScenarioBuilder WithCustomerId(Guid customerId)
    {
        _customerId = customerId;
        return this;
    }

    public AppointmentIntegrationScenarioBuilder WithVehicleId(Guid vehicleId)
    {
        _vehicleId = vehicleId;
        return this;
    }

    public AppointmentIntegrationScenarioBuilder WithServiceTypeId(Guid serviceTypeId)
    {
        _serviceTypeId = serviceTypeId;
        return this;
    }

    public AppointmentIntegrationScenarioBuilder WithTechnicianId(Guid technicianId)
    {
        _technicianId = technicianId;
        return this;
    }

    public AppointmentIntegrationScenarioBuilder WithServiceBayId(Guid serviceBayId)
    {
        _serviceBayId = serviceBayId;
        return this;
    }

    public AppointmentIntegrationScenarioBuilder WithDealership(string name, string address)
    {
        _dealershipName = name;
        _dealershipAddress = address;
        return this;
    }

    public AppointmentIntegrationScenarioBuilder WithCustomer(string firstName, string lastName, string email)
    {
        _customerFirstName = firstName;
        _customerLastName = lastName;
        _customerEmail = email;
        return this;
    }

    public AppointmentIntegrationScenarioBuilder WithVehicle(string make, string model, int year)
    {
        _vehicleMake = make;
        _vehicleModel = model;
        _vehicleYear = year;
        return this;
    }

    public AppointmentIntegrationScenarioBuilder WithService(string name, int durationMinutes)
    {
        _serviceName = name;
        _serviceDurationMinutes = durationMinutes;
        return this;
    }

    public AppointmentIntegrationScenarioBuilder WithTechnician(string firstName, string lastName)
    {
        _technicianFirstName = firstName;
        _technicianLastName = lastName;
        return this;
    }

    public AppointmentIntegrationScenarioBuilder WithServiceBay(string name)
    {
        _serviceBayName = name;
        return this;
    }

    public AppointmentIntegrationScenarioBuilder IncludeExistingAppointments(bool include)
    {
        _includeExistingAppointments = include;
        return this;
    }

    public AppointmentIntegrationScenarioBuilder WithExistingAppointmentCount(int count)
    {
        _existingAppointmentCount = count;
        return this;
    }

    public AppointmentIntegrationScenarioBuilder WithNumberOfTechnicians(int count)
    {
        _numberOfTechnicians = count;
        return this;
    }

    public AppointmentIntegrationScenarioBuilder WithNumberOfServiceBays(int count)
    {
        _numberOfServiceBays = count;
        return this;
    }

    /// <summary>
    /// Builds a complete appointment integration test scenario with all required entities
    /// Returns: (Dealership, Customer, Vehicle, ServiceType, List<Technician>, List<ServiceBay>, List<Appointment>)
    /// </summary>
    public (Dealership, Customer, Vehicle, ServiceType, List<Technician>, List<ServiceBay>, List<Appointment>) Build()
    {
        // Create dealership
        var dealership = new Dealership
        {
            Id = _dealershipId,
            Name = _dealershipName,
            Address = _dealershipAddress
        };

        // Create customer
        var customer = new Customer
        {
            Id = _customerId,
            FirstName = _customerFirstName,
            LastName = _customerLastName,
            Email = _customerEmail
        };

        // Create vehicle
        var vehicle = new Vehicle
        {
            Id = _vehicleId,
            CustomerId = _customerId,
            Make = _vehicleMake,
            Model = _vehicleModel,
            Year = _vehicleYear
        };

        // Create service type
        var serviceType = new ServiceType
        {
            Id = _serviceTypeId,
            Name = _serviceName,
            DurationMinutes = _serviceDurationMinutes
        };

        // Create technicians
        var technicians = new List<Technician>();
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
        }

        // Create service bays
        var serviceBays = new List<ServiceBay>();
        for (int i = 0; i < _numberOfServiceBays; i++)
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
        if (_includeExistingAppointments)
        {
            var now = DateTime.UtcNow;
            for (int i = 0; i < _existingAppointmentCount; i++)
            {
                var appointment = new Appointment
                {
                    Id = Guid.NewGuid(),
                    DealershipId = _dealershipId,
                    CustomerId = Guid.NewGuid(),
                    VehicleId = Guid.NewGuid(),
                    AppointmentDate = DateOnly.FromDateTime(now.AddDays(1).AddHours(i * 2)),
                    StatusId = Guid.Parse("00000000-0000-0000-0000-000000000001") // Booked
                };
                existingAppointments.Add(appointment);
            }
        }

        return (dealership, customer, vehicle, serviceType, technicians, serviceBays, existingAppointments);
    }
}
