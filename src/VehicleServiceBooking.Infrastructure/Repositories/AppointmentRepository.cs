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
/// Repository implementation for Appointment entity persistence operations
/// </summary>
public class AppointmentRepository : IAppointmentRepository
{
    private readonly IApplicationDbContext _dbContext;

    public AppointmentRepository(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<Appointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Appointments
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Appointment>> GetByServiceBayAsync(
        Guid serviceBayId,
        DateTime startTime,
        DateTime endTime,
        CancellationToken cancellationToken)
    {
        // Get appointments that have services assigned to the specified service bay
        // Filter by service timing (EstimatedStartTime/EstimatedEndTime)
        var appointments = await _dbContext.Appointments
            .Where(a => a.Services.Any(s => 
                s.ServiceBayId == serviceBayId &&
                s.EstimatedStartTime >= startTime &&
                s.EstimatedEndTime <= endTime))
            .ToListAsync(cancellationToken);

        return appointments;
    }

    public async Task<IEnumerable<Appointment>> GetByDealershipAsync(
        Guid dealershipId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Appointments
            .Where(a => a.DealershipId == dealershipId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Appointment> AddAsync(Appointment appointment, CancellationToken cancellationToken)
    {
        _dbContext.Appointments.Add(appointment);
        await SaveChangesAsync(cancellationToken);
        return appointment;
    }

    public async Task<Appointment> UpdateAsync(Appointment appointment, CancellationToken cancellationToken)
    {
        _dbContext.Appointments.Update(appointment);
        await SaveChangesAsync(cancellationToken);
        return appointment;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var appointment = await GetByIdAsync(id, cancellationToken);
        if (appointment != null)
        {
            _dbContext.Appointments.Remove(appointment);
            await SaveChangesAsync(cancellationToken);
        }
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
