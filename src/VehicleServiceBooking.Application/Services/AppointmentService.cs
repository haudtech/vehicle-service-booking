using System;
using System.Linq;
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
                "technicianId={TechnicianId}, startTimeSlotId={StartTimeSlotId}, endTimeSlotId={EndTimeSlotId}",
                request.CustomerId, request.VehicleId, request.TechnicianId,
                request.EstimatedStartTimeSlotId, request.EstimatedEndTimeSlotId);

            // Step 1: Validate that all related entities exist
            await ValidateEntitiesExistAsync(request, cancellationToken);

            // Step 2: Check slot availability (re-validate to prevent race conditions)
            await CheckSlotAvailabilityAsync(request, cancellationToken);

            // Step 3: Get the Booked status from database
            var bookedStatus = await _dbContext.AppointmentStatusLookups
                .FirstOrDefaultAsync(s => s.Status == AppointmentStatus.Booked, cancellationToken);
            
            if (bookedStatus == null)
            {
                throw new InvalidOperationException("Booked status not found in database");
            }

            // Step 4: Create appointment entity
            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                DealershipId = request.DealershipId,
                CustomerId = request.CustomerId,
                VehicleId = request.VehicleId,
                AppointmentDate = request.AppointmentDate,
                StatusId = bookedStatus.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Step 5: Create a Service entity for the scheduled service
            // Get the Pending status from database for the new service
            var pendingServiceStatus = await _dbContext.ServiceStatusLookups
                .FirstOrDefaultAsync(s => s.Status == ServiceStatus.Pending, cancellationToken);
            
            if (pendingServiceStatus == null)
            {
                throw new InvalidOperationException("Pending service status not found in database");
            }

            var service = new Service
            {
                Id = Guid.NewGuid(),
                AppointmentId = appointment.Id,
                ServiceTypeId = request.ServiceTypeId,
                TechnicianId = request.TechnicianId,
                ServiceBayId = request.ServiceBayId,
                DealershipId = request.DealershipId,
                ServiceStatusId = pendingServiceStatus.Id,
                SequenceOrder = 1,
                EstimatedStartTimeSlotId = request.EstimatedStartTimeSlotId,  // Set from request
                EstimatedEndTimeSlotId = request.EstimatedEndTimeSlotId,      // Set from request
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Add the service to the appointment's Services collection
            appointment.Services.Add(service);

            // Step 6: Persist to database
            await _appointmentRepository.AddAsync(appointment, cancellationToken);

            _logger.LogInformation(
                "Appointment created successfully: appointmentId={AppointmentId}",
                appointment.Id);

            // Step 7: Return response (calculate times from TimeSlots)
            var startSlot = await _dbContext.TimeSlots.FindAsync(
                new object[] { request.EstimatedStartTimeSlotId }, 
                cancellationToken: cancellationToken);
            
            var endSlot = await _dbContext.TimeSlots.FindAsync(
                new object[] { request.EstimatedEndTimeSlotId }, 
                cancellationToken: cancellationToken);

            var slotStart = startSlot != null 
                ? request.AppointmentDate.ToDateTime(startSlot.SlotStartTime)
                : DateTime.UtcNow;
            
            var slotEnd = endSlot != null 
                ? request.AppointmentDate.ToDateTime(endSlot.SlotEndTime)
                : DateTime.UtcNow;

            return new CreateAppointmentResponse
            {
                AppointmentId = appointment.Id,
                SlotStart = slotStart,
                SlotEnd = slotEnd,
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
                .Include(a => a.Services)
                .FirstOrDefaultAsync(a => a.Id == appointmentId, cancellationToken);

            if (appointment == null)
            {
                _logger.LogWarning("Appointment not found: appointmentId={AppointmentId}", appointmentId);
                return null;
            }

            // Get the earliest and latest TimeSlots from all services
            var services = appointment.Services;
            var startTimeSlotId = services.Min(s => s.EstimatedStartTimeSlotId);
            var endTimeSlotId = services.Max(s => s.EstimatedEndTimeSlotId);

            // Fetch the TimeSlots to calculate DateTime values
            var startSlot = startTimeSlotId.HasValue 
                ? await _dbContext.TimeSlots.FindAsync(new object[] { startTimeSlotId }, cancellationToken: cancellationToken)
                : null;
            
            var endSlot = endTimeSlotId.HasValue 
                ? await _dbContext.TimeSlots.FindAsync(new object[] { endTimeSlotId }, cancellationToken: cancellationToken)
                : null;

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
        _logger.LogDebug("Checking slot availability for vehicle {VehicleId}", request.VehicleId);

        // Get the Cancelled status ID
        var cancelledStatusId = Guid.Parse("00000000-0000-0000-0000-000000000004");

        // Check for conflicting appointments for the same vehicle at overlapping times
        // Query through Services to find overlapping time slots
        var conflictingAppointment = await _dbContext.Appointments
            .Where(a => a.VehicleId == request.VehicleId && a.StatusId != cancelledStatusId && a.AppointmentDate == request.AppointmentDate)
            .Include(a => a.Services)
            .FirstOrDefaultAsync(
                a => a.Services.Any(s => 
                    s.EstimatedStartTimeSlotId.HasValue &&
                    s.EstimatedEndTimeSlotId.HasValue &&
                    s.EstimatedStartTimeSlotId <= request.EstimatedEndTimeSlotId &&
                    s.EstimatedEndTimeSlotId > request.EstimatedStartTimeSlotId),
                cancellationToken);

        if (conflictingAppointment != null)
        {
            var conflictingService = conflictingAppointment.Services.FirstOrDefault(s =>
                s.EstimatedStartTimeSlotId.HasValue &&
                s.EstimatedEndTimeSlotId.HasValue &&
                s.EstimatedStartTimeSlotId <= request.EstimatedEndTimeSlotId &&
                s.EstimatedEndTimeSlotId > request.EstimatedStartTimeSlotId);

            throw new InvalidOperationException(
                $"The selected time slot is no longer available. " +
                $"Another appointment is already booked for this vehicle from slot {conflictingService?.EstimatedStartTimeSlotId} " +
                $"to slot {conflictingService?.EstimatedEndTimeSlotId}");
        }

        _logger.LogDebug("Slot is available");
    }
}
