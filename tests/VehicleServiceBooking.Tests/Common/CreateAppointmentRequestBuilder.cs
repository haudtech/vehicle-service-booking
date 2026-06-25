using VehicleServiceBooking.Application.DTOs;

namespace VehicleServiceBooking.Tests.Common;

/// <summary>
/// Test data builder for CreateAppointmentRequest.
/// Provides a fluent API to create test data with customizable values.
/// </summary>
public class CreateAppointmentRequestBuilder
{
    private Guid _dealershipId = Guid.NewGuid();
    private Guid _customerId = Guid.NewGuid();
    private Guid _vehicleId = Guid.NewGuid();
    private Guid _serviceTypeId = Guid.NewGuid();
    private Guid _technicianId = Guid.NewGuid();
    private Guid _serviceBayId = Guid.NewGuid();
    private DateTime _slotStart = DateTime.UtcNow.AddHours(1);
    private DateTime _slotEnd = DateTime.UtcNow.AddHours(2);

    /// <summary>
    /// Creates a valid request with default values.
    /// </summary>
    public static CreateAppointmentRequestBuilder ValidRequest()
    {
        return new CreateAppointmentRequestBuilder();
    }

    /// <summary>
    /// Sets dealership ID to empty GUID (invalid).
    /// </summary>
    public CreateAppointmentRequestBuilder WithEmptyDealershipId()
    {
        _dealershipId = Guid.Empty;
        return this;
    }

    /// <summary>
    /// Sets dealership ID to a valid random GUID.
    /// </summary>
    public CreateAppointmentRequestBuilder WithValidDealershipId()
    {
        _dealershipId = Guid.NewGuid();
        return this;
    }

    /// <summary>
    /// Sets dealership ID to a specific value.
    /// </summary>
    public CreateAppointmentRequestBuilder WithDealershipId(Guid dealershipId)
    {
        _dealershipId = dealershipId;
        return this;
    }

    /// <summary>
    /// Sets customer ID to empty GUID (invalid).
    /// </summary>
    public CreateAppointmentRequestBuilder WithEmptyCustomerId()
    {
        _customerId = Guid.Empty;
        return this;
    }

    /// <summary>
    /// Sets customer ID to a specific value.
    /// </summary>
    public CreateAppointmentRequestBuilder WithCustomerId(Guid customerId)
    {
        _customerId = customerId;
        return this;
    }

    /// <summary>
    /// Sets vehicle ID to empty GUID (invalid).
    /// </summary>
    public CreateAppointmentRequestBuilder WithEmptyVehicleId()
    {
        _vehicleId = Guid.Empty;
        return this;
    }

    /// <summary>
    /// Sets vehicle ID to a specific value.
    /// </summary>
    public CreateAppointmentRequestBuilder WithVehicleId(Guid vehicleId)
    {
        _vehicleId = vehicleId;
        return this;
    }

    /// <summary>
    /// Sets service type ID to empty GUID (invalid).
    /// </summary>
    public CreateAppointmentRequestBuilder WithEmptyServiceTypeId()
    {
        _serviceTypeId = Guid.Empty;
        return this;
    }

    /// <summary>
    /// Sets service type ID to a specific value.
    /// </summary>
    public CreateAppointmentRequestBuilder WithServiceTypeId(Guid serviceTypeId)
    {
        _serviceTypeId = serviceTypeId;
        return this;
    }

    /// <summary>
    /// Sets technician ID to empty GUID (invalid).
    /// </summary>
    public CreateAppointmentRequestBuilder WithEmptyTechnicianId()
    {
        _technicianId = Guid.Empty;
        return this;
    }

    /// <summary>
    /// Sets technician ID to a specific value.
    /// </summary>
    public CreateAppointmentRequestBuilder WithTechnicianId(Guid technicianId)
    {
        _technicianId = technicianId;
        return this;
    }

    /// <summary>
    /// Sets service bay ID to empty GUID (invalid).
    /// </summary>
    public CreateAppointmentRequestBuilder WithEmptyServiceBayId()
    {
        _serviceBayId = Guid.Empty;
        return this;
    }

    /// <summary>
    /// Sets service bay ID to a specific value.
    /// </summary>
    public CreateAppointmentRequestBuilder WithServiceBayId(Guid serviceBayId)
    {
        _serviceBayId = serviceBayId;
        return this;
    }

    /// <summary>
    /// Sets both slot start and end times. 
    /// If only start time is provided, end time defaults to 1 hour later.
    /// </summary>
    public CreateAppointmentRequestBuilder WithSlotTimes(DateTime slotStart, DateTime slotEnd)
    {
        _slotStart = slotStart;
        _slotEnd = slotEnd;
        return this;
    }

    /// <summary>
    /// Sets slot start time (end time automatically adjusted to 1 hour later).
    /// </summary>
    public CreateAppointmentRequestBuilder WithSlotStart(DateTime slotStart)
    {
        _slotStart = slotStart;
        _slotEnd = slotStart.AddHours(1);
        return this;
    }

    /// <summary>
    /// Sets slot end time before start time (invalid scenario).
    /// </summary>
    public CreateAppointmentRequestBuilder WithEndTimeBeforeStartTime()
    {
        _slotEnd = _slotStart.AddMinutes(-30);
        return this;
    }

    /// <summary>
    /// Sets slot end time equal to start time (invalid scenario).
    /// </summary>
    public CreateAppointmentRequestBuilder WithEndTimeEqualToStartTime()
    {
        _slotEnd = _slotStart;
        return this;
    }

    /// <summary>
    /// Sets slot times to the past (invalid scenario).
    /// </summary>
    public CreateAppointmentRequestBuilder WithPastSlotTimes()
    {
        _slotStart = DateTime.UtcNow.AddHours(-2);
        _slotEnd = DateTime.UtcNow.AddHours(-1);
        return this;
    }

    /// <summary>
    /// Builds and returns the CreateAppointmentRequest with current values.
    /// </summary>
    public CreateAppointmentRequest Build()
    {
        return new CreateAppointmentRequest
        {
            DealershipId = _dealershipId,
            CustomerId = _customerId,
            VehicleId = _vehicleId,
            ServiceTypeId = _serviceTypeId,
            TechnicianId = _technicianId,
            ServiceBayId = _serviceBayId,
            SlotStart = _slotStart,
            SlotEnd = _slotEnd
        };
    }
}
