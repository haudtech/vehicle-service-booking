using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VehicleServiceBooking.Application.DTOs;
using VehicleServiceBooking.Application.Interfaces;
using VehicleServiceBooking.Application.Interfaces.Persistence;
using VehicleServiceBooking.Application.Interfaces.Repositories;
using VehicleServiceBooking.Application.Interfaces.Services;
using VehicleServiceBooking.Domain.Entities;
using VehicleServiceBooking.Domain.Enums;

namespace VehicleServiceBooking.Application.Services;

/// <summary>
/// Service for managing appointment operations
/// </summary>
public class AppointmentService : IAppointmentService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IAvailabilityService _availabilityService;
    private readonly ILogger<AppointmentService> _logger;

    public AppointmentService(
        IApplicationDbContext dbContext,
        IAppointmentRepository appointmentRepository,
        IAvailabilityService availabilityService,
        ILogger<AppointmentService> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _appointmentRepository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository));
        _availabilityService = availabilityService ?? throw new ArgumentNullException(nameof(availabilityService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Creates a new appointment booking
    /// </summary>
    public async Task<CreateAppointmentResponse> CreateAppointmentAsync(
        CreateAppointmentRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation(
                "Creating appointment: customerId={CustomerId}, vehicleId={VehicleId}, " +
                "technicianId={TechnicianId}, slotStart={SlotStart}, slotEnd={SlotEnd}",
                request.CustomerId, request.VehicleId, request.TechnicianId,
                request.SlotStart, request.SlotEnd);

            // Step 1: Validate that all related entities exist
            await ValidateEntitiesExistAsync(request, cancellationToken);

            // Step 2: Check slot availability (re-validate to prevent race conditions)
            await CheckSlotAvailabilityAsync(request, cancellationToken);

            // Step 3: Create appointment entity
            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                DealershipId = request.DealershipId,
                CustomerId = request.CustomerId,
                VehicleId = request.VehicleId,
                ServiceTypeId = request.ServiceTypeId,
                TechnicianId = request.TechnicianId,
                ServiceBayId = request.ServiceBayId,
                StartTime = request.SlotStart,
                EndTime = request.SlotEnd,
                Status = AppointmentStatus.Booked
            };

            // Step 4: Persist to database
            await _appointmentRepository.AddAsync(appointment, cancellationToken);

            _logger.LogInformation(
                "Appointment created successfully: appointmentId={AppointmentId}",
                appointment.Id);

            // Step 5: Return response
            return new CreateAppointmentResponse
            {
                AppointmentId = appointment.Id,
                SlotStart = appointment.StartTime,
                SlotEnd = appointment.EndTime,
                CreatedAt = DateTime.UtcNow
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
    /// Retrieves an appointment by ID
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

            var appointment = await _dbContext.Appointments
                .FirstOrDefaultAsync(a => a.Id == appointmentId, cancellationToken);

            if (appointment == null)
            {
                _logger.LogWarning("Appointment not found: appointmentId={AppointmentId}", appointmentId);
                return null;
            }

            return new CreateAppointmentResponse
            {
                AppointmentId = appointment.Id,
                SlotStart = appointment.StartTime,
                SlotEnd = appointment.EndTime,
                CreatedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving appointment");
            throw;
        }
    }

    /// <summary>
    /// Validates that all referenced entities exist in the database
    /// </summary>
    private async Task ValidateEntitiesExistAsync(
        CreateAppointmentRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("Validating referenced entities exist");

        // Check Customer exists
        var customerExists = await _dbContext.DbContext.Set<Customer>()
            .AnyAsync(c => c.Id == request.CustomerId, cancellationToken);
        if (!customerExists)
        {
            throw new InvalidOperationException(
                $"Customer with ID {request.CustomerId} not found");
        }

        // Check Vehicle exists
        var vehicleExists = await _dbContext.DbContext.Set<Vehicle>()
            .AnyAsync(v => v.Id == request.VehicleId, cancellationToken);
        if (!vehicleExists)
        {
            throw new InvalidOperationException(
                $"Vehicle with ID {request.VehicleId} not found");
        }

        // Check ServiceType exists
        var serviceTypeExists = await _dbContext.ServiceTypes
            .AnyAsync(st => st.Id == request.ServiceTypeId, cancellationToken);
        if (!serviceTypeExists)
        {
            throw new InvalidOperationException(
                $"Service type with ID {request.ServiceTypeId} not found");
        }

        // Check Technician exists
        var technicianExists = await _dbContext.DbContext.Set<Technician>()
            .AnyAsync(t => t.Id == request.TechnicianId, cancellationToken);
        if (!technicianExists)
        {
            throw new InvalidOperationException(
                $"Technician with ID {request.TechnicianId} not found");
        }

        // Check ServiceBay exists
        var serviceBayExists = await _dbContext.ServiceBays
            .AnyAsync(sb => sb.Id == request.ServiceBayId, cancellationToken);
        if (!serviceBayExists)
        {
            throw new InvalidOperationException(
                $"Service bay with ID {request.ServiceBayId} not found");
        }

        _logger.LogDebug("All referenced entities validated");
    }

    /// <summary>
    /// Checks if the requested time slot is available
    /// </summary>
    private async Task CheckSlotAvailabilityAsync(
        CreateAppointmentRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("Checking slot availability for service bay {ServiceBayId}", request.ServiceBayId);

        // Check for conflicting appointments in the same service bay
        var conflictingAppointment = await _dbContext.Appointments
            .FirstOrDefaultAsync(
                a => a.ServiceBayId == request.ServiceBayId &&
                     a.Status != AppointmentStatus.Cancelled &&
                     // Check for time overlap
                     a.StartTime < request.SlotEnd &&
                     a.EndTime > request.SlotStart,
                cancellationToken);

        if (conflictingAppointment != null)
        {
            throw new InvalidOperationException(
                $"The selected time slot is no longer available. " +
                $"Another appointment is already booked from {conflictingAppointment.StartTime:g} " +
                $"to {conflictingAppointment.EndTime:g}");
        }

        _logger.LogDebug("Slot is available");
    }
}
