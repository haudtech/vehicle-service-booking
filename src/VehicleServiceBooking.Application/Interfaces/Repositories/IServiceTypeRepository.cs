using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VehicleServiceBooking.Domain.Entities;

namespace VehicleServiceBooking.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for ServiceType entity.
/// </summary>
public interface IServiceTypeRepository : IReadRepository<ServiceType>, IWriteRepository<ServiceType>
{
    /// <summary>
    /// Gets service types by IDs including currency-specific price rows.
    /// </summary>
    Task<IEnumerable<ServiceType>> GetByIdsWithPricesAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken);
}
