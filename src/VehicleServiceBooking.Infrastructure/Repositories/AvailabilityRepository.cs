using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VehicleServiceBooking.Application.Interfaces.Persistence;
using VehicleServiceBooking.Application.Interfaces.Repositories;
using VehicleServiceBooking.Domain.Entities;

namespace VehicleServiceBooking.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for availability checking and slot management
/// </summary>
public class AvailabilityRepository : IAvailabilityRepository
{
    private readonly IApplicationDbContext _dbContext;

    public AvailabilityRepository(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<IEnumerable<Appointment>> GetAppointmentsByServiceTypeAsync(
        Guid serviceTypeId,
        DateTime startTime,
        DateTime endTime,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Appointments
            .Where(a => a.ServiceTypeId == serviceTypeId
                && a.StartTime >= startTime
                && a.EndTime <= endTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Technician>> GetTechniciansByDealershipAsync(
        Guid dealershipId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Technicians
            .Where(t => t.DealershipId == dealershipId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Technician>> GetQualifiedTechniciansAsync(
        Guid serviceTypeId,
        Guid dealershipId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Technicians
            .Where(t => t.DealershipId == dealershipId
                && t.Skills.Any(s => s.ServiceTypeId == serviceTypeId))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ServiceBay>> GetServiceBaysAsync(
        Guid dealershipId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.ServiceBays
            .Where(s => s.DealershipId == dealershipId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Appointment>> GetAppointmentsInRangeAsync(
        DateTime startTime,
        DateTime endTime,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Appointments
            .Where(a => a.StartTime >= startTime && a.EndTime <= endTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<BusinessHours>> GetBusinessHoursAsync(
        Guid dealershipId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.BusinessHours
            .Where(b => b.DealershipId == dealershipId)
            .ToListAsync(cancellationToken);
    }
}
