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
    private DateOnly _appointmentDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
    private Guid _startTimeSlotId = Guid.Parse("00000000-0000-0000-0000-000000000001");  // Slot 1: 08:00-08:30
    private Guid _endTimeSlotId = Guid.Parse("00000000-0000-0000-0000-000000000002");    // Slot 2: 08:30-09:00

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
    /// Sets the appointment date.
    /// </summary>
    public CreateAppointmentRequestBuilder WithAppointmentDate(DateOnly appointmentDate)
    {
        _appointmentDate = appointmentDate;
        return this;
    }

    /// <summary>
    /// Sets both start and end time slot IDs.
    /// </summary>
    public CreateAppointmentRequestBuilder WithTimeSlots(Guid startTimeSlotId, Guid endTimeSlotId)
    {
        _startTimeSlotId = startTimeSlotId;
        _endTimeSlotId = endTimeSlotId;
        return this;
    }

    /// <summary>
    /// Sets the start time slot ID.
    /// </summary>
    public CreateAppointmentRequestBuilder WithStartTimeSlotId(Guid startTimeSlotId)
    {
        _startTimeSlotId = startTimeSlotId;
        return this;
    }

    /// <summary>
    /// Sets the end time slot ID.
    /// </summary>
    public CreateAppointmentRequestBuilder WithEndTimeSlotId(Guid endTimeSlotId)
    {
        _endTimeSlotId = endTimeSlotId;
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
            AppointmentDate = _appointmentDate,
            ServiceTypeId = _serviceTypeId,
            TechnicianId = _technicianId,
            ServiceBayId = _serviceBayId,
            EstimatedStartTimeSlotId = _startTimeSlotId,
            EstimatedEndTimeSlotId = _endTimeSlotId
        };
    }
}
