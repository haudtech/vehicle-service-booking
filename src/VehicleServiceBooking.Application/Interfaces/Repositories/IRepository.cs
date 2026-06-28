namespace VehicleServiceBooking.Application.Interfaces.Repositories;

/// <summary>
/// Base marker interface for all repository contracts.
/// </summary>
/// <typeparam name="TEntity">The entity type managed by the repository.</typeparam>
public interface IRepository<TEntity> where TEntity : class
{
}