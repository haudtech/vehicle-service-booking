using VehicleServiceBooking.Application.Models;
using VehicleServiceBooking.Domain.Entities;
using AppTimeSlot = VehicleServiceBooking.Application.Models.DateTimeSlot;

namespace VehicleServiceBooking.Tests.Common;

public class AvailabilityProjectionBuilder
{
    private Guid _timeSlotId = Guid.NewGuid();
    private TimeOnly _slotStartTime = new(8, 0);
    private TimeOnly _slotEndTime = new(8, 30);
    private Guid _technicianId = Guid.NewGuid();
    private Guid _serviceBayId = Guid.NewGuid();

    public static AvailabilityProjectionBuilder CreateValid() => new();

    public AvailabilityProjectionBuilder WithTimeSlotId(Guid id)
    {
        _timeSlotId = id;
        return this;
    }

    public AvailabilityProjectionBuilder WithSlotTimes(TimeOnly start, TimeOnly end)
    {
        _slotStartTime = start;
        _slotEndTime = end;
        return this;
    }

    public AvailabilityProjectionBuilder WithTechnicianId(Guid id)
    {
        _technicianId = id;
        return this;
    }

    public AvailabilityProjectionBuilder WithServiceBayId(Guid id)
    {
        _serviceBayId = id;
        return this;
    }

    public AvailabilityProjection Build()
    {
        return new AvailabilityProjection
        {
            TimeSlotId = _timeSlotId,
            SlotStartTime = _slotStartTime,
            SlotEndTime = _slotEndTime,
            TechnicianId = _technicianId,
            ServiceBayId = _serviceBayId
        };
    }
}

public class AppTimeSlotBuilder
{
    private DateTime _start = DateTime.UtcNow.Date.AddHours(8);
    private DateTime _end = DateTime.UtcNow.Date.AddHours(8).AddMinutes(30);

    public static AppTimeSlotBuilder CreateValid() => new();

    public AppTimeSlotBuilder WithTimes(TimeOnly start, TimeOnly end)
    {
        var baseDate = DateTime.UtcNow.Date;
        _start = baseDate.Add(start.ToTimeSpan());
        _end = baseDate.Add(end.ToTimeSpan());
        return this;
    }

    public AppTimeSlotBuilder WithDateTimeRange(DateTime start, DateTime end)
    {
        _start = start;
        _end = end;
        return this;
    }

    public AppTimeSlot Build()
    {
        return new AppTimeSlot
        {
            Start = _start,
            End = _end
        };
    }
}

public class AvailabilityOptionBuilder
{
    private AppTimeSlot _timeSlot = AppTimeSlotBuilder.CreateValid().Build();
    private Guid _technicianId = Guid.NewGuid();
    private Guid _serviceBayId = Guid.NewGuid();

    public static AvailabilityOptionBuilder CreateValid() => new();

    public AvailabilityOptionBuilder WithTimeSlot(AppTimeSlot timeSlot)
    {
        _timeSlot = timeSlot;
        return this;
    }

    public AvailabilityOptionBuilder WithTechnicianId(Guid id)
    {
        _technicianId = id;
        return this;
    }

    public AvailabilityOptionBuilder WithServiceBayId(Guid id)
    {
        _serviceBayId = id;
        return this;
    }

    public AvailabilityOption Build()
    {
        return new AvailabilityOption
        {
            DateTimeSlot = _timeSlot,
            TechnicianId = _technicianId,
            ServiceBayId = _serviceBayId
        };
    }
}

public class DomainTimeSlotBuilder
{
    private Guid _id = Guid.NewGuid();
    private int _sequenceOrder = 1;
    private TimeOnly _slotStartTime = new(8, 0);
    private TimeOnly _slotEndTime = new(8, 30);

    public static DomainTimeSlotBuilder CreateValid() => new();

    public DomainTimeSlotBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public DomainTimeSlotBuilder WithSequenceOrder(int sequenceOrder)
    {
        _sequenceOrder = sequenceOrder;
        return this;
    }

    public DomainTimeSlotBuilder WithSlotTimes(TimeOnly start, TimeOnly end)
    {
        _slotStartTime = start;
        _slotEndTime = end;
        return this;
    }

    public VehicleServiceBooking.Domain.Entities.TimeSlot Build()
    {
        return new VehicleServiceBooking.Domain.Entities.TimeSlot
        {
            Id = _id,
            SequenceOrder = _sequenceOrder,
            SlotStartTime = _slotStartTime,
            SlotEndTime = _slotEndTime
        };
    }
}

public class ServiceEntityBuilder
{
    private Guid _id = Guid.NewGuid();
    private Guid _appointmentId = Guid.NewGuid();
    private Guid _serviceTypeId = Guid.NewGuid();
    private Guid? _technicianId = Guid.NewGuid();
    private Guid? _serviceBayId = Guid.NewGuid();
    private Guid _dealershipId = Guid.NewGuid();
    private Guid _serviceStatusId = Guid.Parse("00000000-0000-0000-0001-000000000001");
    private int _sequenceOrder = 1;
    private Guid? _estimatedStartTimeSlotId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private Guid? _estimatedEndTimeSlotId = Guid.Parse("00000000-0000-0000-0000-000000000002");

    public static ServiceEntityBuilder CreateValid() => new();

    public ServiceEntityBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public ServiceEntityBuilder WithAppointmentId(Guid appointmentId)
    {
        _appointmentId = appointmentId;
        return this;
    }

    public ServiceEntityBuilder WithServiceTypeId(Guid serviceTypeId)
    {
        _serviceTypeId = serviceTypeId;
        return this;
    }

    public ServiceEntityBuilder WithTechnicianId(Guid? technicianId)
    {
        _technicianId = technicianId;
        return this;
    }

    public ServiceEntityBuilder WithServiceBayId(Guid? serviceBayId)
    {
        _serviceBayId = serviceBayId;
        return this;
    }

    public ServiceEntityBuilder WithDealershipId(Guid dealershipId)
    {
        _dealershipId = dealershipId;
        return this;
    }

    public ServiceEntityBuilder WithSequenceOrder(int sequenceOrder)
    {
        _sequenceOrder = sequenceOrder;
        return this;
    }

    public ServiceEntityBuilder WithEstimatedSlotIds(Guid? startSlotId, Guid? endSlotId)
    {
        _estimatedStartTimeSlotId = startSlotId;
        _estimatedEndTimeSlotId = endSlotId;
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
            SequenceOrder = _sequenceOrder,
            EstimatedStartTimeSlotId = _estimatedStartTimeSlotId,
            EstimatedEndTimeSlotId = _estimatedEndTimeSlotId
        };
    }
}

public class AppointmentEntityBuilder
{
    private Guid _id = Guid.NewGuid();
    private Guid _dealershipId = Guid.NewGuid();
    private Guid _customerId = Guid.NewGuid();
    private Guid _vehicleId = Guid.NewGuid();
    private DateOnly _appointmentDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
    private Guid _statusId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private DateTime _createdAt = DateTime.UtcNow;
    private DateTime _updatedAt = DateTime.UtcNow;
    private List<Service> _services = [];

    public static AppointmentEntityBuilder CreateValid() => new();

    public AppointmentEntityBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public AppointmentEntityBuilder WithDealershipId(Guid dealershipId)
    {
        _dealershipId = dealershipId;
        return this;
    }

    public AppointmentEntityBuilder WithCustomerId(Guid customerId)
    {
        _customerId = customerId;
        return this;
    }

    public AppointmentEntityBuilder WithVehicleId(Guid vehicleId)
    {
        _vehicleId = vehicleId;
        return this;
    }

    public AppointmentEntityBuilder WithAppointmentDate(DateOnly appointmentDate)
    {
        _appointmentDate = appointmentDate;
        return this;
    }

    public AppointmentEntityBuilder WithStatusId(Guid statusId)
    {
        _statusId = statusId;
        return this;
    }

    public AppointmentEntityBuilder WithTimestamps(DateTime createdAt, DateTime updatedAt)
    {
        _createdAt = createdAt;
        _updatedAt = updatedAt;
        return this;
    }

    public AppointmentEntityBuilder WithServices(IEnumerable<Service> services)
    {
        _services = services.ToList();
        return this;
    }

    public Appointment Build()
    {
        return new Appointment
        {
            Id = _id,
            DealershipId = _dealershipId,
            CustomerId = _customerId,
            VehicleId = _vehicleId,
            AppointmentDate = _appointmentDate,
            StatusId = _statusId,
            CreatedAt = _createdAt,
            UpdatedAt = _updatedAt,
            Services = _services
        };
    }
}
