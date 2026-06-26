using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VehicleServiceBooking.Application.DTOs;
using VehicleServiceBooking.Application.Interfaces.Repositories;
using VehicleServiceBooking.Application.Interfaces.Services;
using VehicleServiceBooking.Domain.Entities;
using VehicleServiceBooking.Domain.Enums;

namespace VehicleServiceBooking.Application.Services;

/// <summary>
/// Service for managing appointment operations.
/// Implements business logic layer - all data access delegated to Repository layer.
/// </summary>
public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IAvailabilityService _availabilityService;
    private readonly ILogger<AppointmentService> _logger;

    /// <summary>
    /// Constructor enforcing Repository pattern: Service depends on Repository, not dbContext
    /// </summary>
    public AppointmentService(
        IAppointmentRepository appointmentRepository,
        IAvailabilityService availabilityService,
        ILogger<AppointmentService> logger)
    {
        _appointmentRepository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository));
        _availabilityService = availabilityService ?? throw new ArgumentNullException(nameof(availabilityService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Creates a new appointment booking with business logic validation.
    /// All data access delegated to Repository layer.
    /// </summary>
    public async Task<CreateAppointmentResponse> CreateAppointmentAsync(
        CreateAppointmentRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation(
                "Creating appointment: customerId={CustomerId}, vehicleId={VehicleId}, " +
                "technicianId={TechnicianId}, startTimeSlotId={StartTimeSlotId}, endTimeSlotId={EndTimeSlotId}",
                request.CustomerId, request.VehicleId, request.TechnicianId,
                request.EstimatedStartTimeSlotId, request.EstimatedEndTimeSlotId);

            // BUSINESS LOGIC 1: Validate availability via AvailabilityService
            // This provides business-level validation of time slots across all constraints
            _logger.LogDebug("Step 1: Validating availability via AvailabilityService");
            var availabilityDate = request.AppointmentDate.ToDateTime(TimeOnly.MinValue);
            
            var availabilityOptions = await _availabilityService.GetAvailableSlotsAsync(
                request.DealershipId,
                request.ServiceTypeId,
                availabilityDate,
                cancellationToken);

            if (!availabilityOptions.Any())
            {
                throw new InvalidOperationException(
                    "No available slots matching the specified criteria");
            }

            // BUSINESS LOGIC 2: Validate requested slot is in available slots
            var requestedSlotAvailable = availabilityOptions.Any(slot =>
                slot.TechnicianId == request.TechnicianId &&
                slot.ServiceBayId == request.ServiceBayId);

            if (!requestedSlotAvailable)
            {
                throw new InvalidOperationException(
                    $"The selected technician and service bay combination is not in the available slots");
            }

            // BUSINESS LOGIC 3: Verify technician has required skill (data query)
            _logger.LogDebug("Step 2: Verifying technician has required skill");
            var technicianHasSkill = await _appointmentRepository.TechnicianHasSkillAsync(
                request.TechnicianId,
                request.ServiceTypeId,
                cancellationToken);

            if (!technicianHasSkill)
            {
                throw new InvalidOperationException(
                    $"Technician {request.TechnicianId} does not have the required skill for service type {request.ServiceTypeId}");
            }

            // BUSINESS LOGIC 4: Check for vehicle conflicts (re-validate to prevent race conditions)
            _logger.LogDebug("Step 3: Checking for vehicle appointment conflicts");
            await CheckVehicleConflictsAsync(request, cancellationToken);

            // BUSINESS LOGIC 5: Create appointment with services (atomic transaction via Repository)
            _logger.LogDebug("Step 4: Creating appointment and service entities");
            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                DealershipId = request.DealershipId,
                CustomerId = request.CustomerId,
                VehicleId = request.VehicleId,
                AppointmentDate = request.AppointmentDate,
                StatusId = Guid.Parse("00000000-0000-0000-0000-000000000003"), // Booked status
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Create service entity
            var service = new Service
            {
                Id = Guid.NewGuid(),
                AppointmentId = appointment.Id,
                ServiceTypeId = request.ServiceTypeId,
                TechnicianId = request.TechnicianId,
                ServiceBayId = request.ServiceBayId,
                DealershipId = request.DealershipId,
                ServiceStatusId = Guid.Parse("00000000-0000-0000-0000-000000000006"), // Pending status
                SequenceOrder = 1,
                EstimatedStartTimeSlotId = request.EstimatedStartTimeSlotId,
                EstimatedEndTimeSlotId = request.EstimatedEndTimeSlotId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            appointment.Services.Add(service);

            // BUSINESS LOGIC 6: Persist atomically via Repository
            _logger.LogDebug("Step 5: Persisting appointment with service to repository");
            var createdAppointment = await _appointmentRepository.CreateAppointmentWithServicesAsync(
                appointment, cancellationToken);

            _logger.LogInformation(
                "Appointment created successfully: appointmentId={AppointmentId}",
                createdAppointment.Id);

            // BUSINESS LOGIC 7: Load time slot details for response (data query)
            _logger.LogDebug("Step 6: Loading time slot details for response");
            var startTimeSlot = await _appointmentRepository.GetByIdAsync(createdAppointment.Id, cancellationToken)
                .ConfigureAwait(false);
            
            if (startTimeSlot?.Services.FirstOrDefault() is not Service firstService ||
                !firstService.EstimatedStartTimeSlotId.HasValue ||
                !firstService.EstimatedEndTimeSlotId.HasValue)
            {
                throw new InvalidOperationException("Failed to retrieve created appointment details");
            }

            // Load the actual TimeSlot entities by ID to get time values
            var timeSlotIds = new[] { firstService.EstimatedStartTimeSlotId.Value, firstService.EstimatedEndTimeSlotId.Value };
            var timeSlots = await _appointmentRepository.GetTimeSlotsBySequenceRangeAsync(
                0, int.MaxValue, cancellationToken).ConfigureAwait(false);
            
            var startSlot = timeSlots.FirstOrDefault(ts => ts.Id == firstService.EstimatedStartTimeSlotId.Value);
            var endSlot = timeSlots.FirstOrDefault(ts => ts.Id == firstService.EstimatedEndTimeSlotId.Value);

            var slotStart = startSlot != null
                ? request.AppointmentDate.ToDateTime(startSlot.SlotStartTime)
                : DateTime.UtcNow;

            var slotEnd = endSlot != null
                ? request.AppointmentDate.ToDateTime(endSlot.SlotEndTime)
                : DateTime.UtcNow;

            return new CreateAppointmentResponse
            {
                AppointmentId = createdAppointment.Id,
                SlotStart = slotStart,
                SlotEnd = slotEnd,
                CreatedAt = createdAppointment.CreatedAt
            };
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation in CreateAppointmentAsync: {Message}", ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating appointment");
            throw new InvalidOperationException("Failed to create appointment", ex);
        }
    }

    /// <summary>
    /// Retrieves an appointment by ID.
    /// All data queries delegated to Repository layer.
    /// </summary>
    public async Task<CreateAppointmentResponse?> GetAppointmentByIdAsync(
        Guid appointmentId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (appointmentId == Guid.Empty)
            {
                throw new ArgumentException("Appointment ID cannot be empty", nameof(appointmentId));
            }

            _logger.LogInformation("Retrieving appointment: appointmentId={AppointmentId}", appointmentId);

            // DATA QUERY: Retrieve appointment from Repository
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);

            if (appointment == null)
            {
                _logger.LogWarning("Appointment not found: appointmentId={AppointmentId}", appointmentId);
                return null;
            }

            // BUSINESS LOGIC: Calculate slot times from services
            var services = appointment.Services;
            if (!services.Any())
            {
                _logger.LogWarning("Appointment has no services: appointmentId={AppointmentId}", appointmentId);
                return null;
            }

            var startTimeSlotId = services.Min(s => s.EstimatedStartTimeSlotId);
            var endTimeSlotId = services.Max(s => s.EstimatedEndTimeSlotId);

            // DATA QUERY: Load time slot details
            if (!startTimeSlotId.HasValue || !endTimeSlotId.HasValue)
            {
                _logger.LogWarning(
                    "Appointment services missing time slot IDs: appointmentId={AppointmentId}",
                    appointmentId);
                return null;
            }

            // Load all timeSlots to find the ones we need
            var timeSlots = await _appointmentRepository.GetTimeSlotsBySequenceRangeAsync(
                0, int.MaxValue, cancellationToken);

            var startSlot = timeSlots.FirstOrDefault(ts => ts.Id == startTimeSlotId.Value);
            var endSlot = timeSlots.FirstOrDefault(ts => ts.Id == endTimeSlotId.Value);

            var slotStart = startSlot != null
                ? appointment.AppointmentDate.ToDateTime(startSlot.SlotStartTime)
                : DateTime.UtcNow;

            var slotEnd = endSlot != null
                ? appointment.AppointmentDate.ToDateTime(endSlot.SlotEndTime)
                : DateTime.UtcNow;

            return new CreateAppointmentResponse
            {
                AppointmentId = appointment.Id,
                SlotStart = slotStart,
                SlotEnd = slotEnd,
                CreatedAt = appointment.CreatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving appointment: appointmentId={AppointmentId}", appointmentId);
            throw;
        }
    }

    /// <summary>
    /// Checks for conflicting appointments on the same vehicle at overlapping times.
    /// Pure business logic validation - no direct data access, uses Repository methods where needed.
    /// </summary>
    private async Task CheckVehicleConflictsAsync(
        CreateAppointmentRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug(
            "Checking for vehicle conflicts: vehicleId={VehicleId}, date={AppointmentDate}, slots={StartSlot}-{EndSlot}",
            request.VehicleId, request.AppointmentDate,
            request.EstimatedStartTimeSlotId, request.EstimatedEndTimeSlotId);

        // DATA QUERY: Get all appointments for this vehicle on the same date
        var vehicleAppointments = await _appointmentRepository.GetByVehicleIdAsync(
            request.VehicleId, cancellationToken);

        // BUSINESS LOGIC: Check for overlapping appointments on same date
        var appointmentCancelledStatusId = Guid.Parse("00000000-0000-0000-0000-000000000004");

        var conflictingAppointment = vehicleAppointments
            .Where(a => a.AppointmentDate == request.AppointmentDate && a.StatusId != appointmentCancelledStatusId)
            .FirstOrDefault(a => a.Services.Any(s =>
                s.EstimatedStartTimeSlotId.HasValue &&
                s.EstimatedEndTimeSlotId.HasValue &&
                s.EstimatedStartTimeSlotId <= request.EstimatedEndTimeSlotId &&
                s.EstimatedEndTimeSlotId > request.EstimatedStartTimeSlotId));

        if (conflictingAppointment != null)
        {
            var conflictingService = conflictingAppointment.Services.FirstOrDefault(s =>
                s.EstimatedStartTimeSlotId.HasValue &&
                s.EstimatedEndTimeSlotId.HasValue &&
                s.EstimatedStartTimeSlotId <= request.EstimatedEndTimeSlotId &&
                s.EstimatedEndTimeSlotId > request.EstimatedStartTimeSlotId);

            throw new InvalidOperationException(
                $"The selected time slot conflicts with an existing appointment. " +
                $"Another appointment is booked from slot {conflictingService?.EstimatedStartTimeSlotId} " +
                $"to slot {conflictingService?.EstimatedEndTimeSlotId}");
        }

        _logger.LogDebug("No vehicle conflicts found");
    }
}
