using VehicleServiceBooking.Domain.Entities;

namespace VehicleServiceBooking.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for supported currency lookup entities.
/// </summary>
public interface ICurrencyLookupRepository : IReadRepository<CurrencyLookup>, IWriteRepository<CurrencyLookup>
{
}