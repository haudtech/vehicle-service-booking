using VehicleServiceBooking.Application.Interfaces.Persistence;
using VehicleServiceBooking.Application.Interfaces.Repositories;
using VehicleServiceBooking.Domain.Entities;

namespace VehicleServiceBooking.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for supported currency lookup entities.
/// </summary>
public sealed class CurrencyLookupRepository : GenericRepository<CurrencyLookup>, ICurrencyLookupRepository
{
    public CurrencyLookupRepository(IApplicationDbContext dbContext)
        : base(dbContext)
    {
    }
}