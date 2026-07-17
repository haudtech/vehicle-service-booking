using System;
using System.Threading;
using System.Threading.Tasks;
using VehicleServiceBooking.Application.Interfaces.Persistence;
using VehicleServiceBooking.Application.Interfaces.Repositories;
using VehicleServiceBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace VehicleServiceBooking.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Customer entity.
/// </summary>
public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(IApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public Task<Customer?> GetByAuthUserIdAsync(Guid authUserId, CancellationToken cancellationToken = default)
    {
        return GetQueryable().FirstOrDefaultAsync(c => c.AuthUserId == authUserId, cancellationToken);
    }
}
