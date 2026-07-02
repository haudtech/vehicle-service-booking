using System;
using System.Threading;
using System.Threading.Tasks;
using VehicleServiceBooking.Domain.Entities;
using VehicleServiceBooking.Domain.Enums;

namespace VehicleServiceBooking.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for AppointmentStatusLookup entity.
/// </summary>
public interface IAppointmentStatusLookupRepository : IReadRepository<AppointmentStatusLookup>
{
    Task<AppointmentStatusLookup?> GetByStatusAsync(AppointmentStatus status, CancellationToken cancellationToken);

    Task<Guid?> GetStatusIdByStatusAsync(AppointmentStatus status, CancellationToken cancellationToken);
}
