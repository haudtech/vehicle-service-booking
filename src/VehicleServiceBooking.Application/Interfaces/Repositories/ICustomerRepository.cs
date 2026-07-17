using System;
using System.Threading;
using System.Threading.Tasks;
using VehicleServiceBooking.Domain.Entities;

namespace VehicleServiceBooking.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for Customer entity.
/// </summary>
public interface ICustomerRepository : IReadRepository<Customer>, IWriteRepository<Customer>
{
	/// <summary>
	/// Gets a customer by auth service user identifier.
	/// </summary>
	Task<Customer?> GetByAuthUserIdAsync(Guid authUserId, CancellationToken cancellationToken = default);
}
