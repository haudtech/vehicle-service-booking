using VehicleServiceBooking.Domain.Entities;

namespace VehicleServiceBooking.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for Customer entity.
/// </summary>
public interface ICustomerRepository : IReadRepository<Customer>, IWriteRepository<Customer>
{
}
