namespace VehicleServiceBooking.Domain.Entities;

/// <summary>
/// Abstract base entity class for all domain entities.
/// Provides common audit fields that all entities should have.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Unique identifier for the entity
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Timestamp when the entity was created (UTC)
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Timestamp when the entity was last updated (UTC)
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Indicates whether the entity is active.
    /// False means the entity is soft-deleted/inactive.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Protected constructor that initializes audit fields with current UTC time
    /// </summary>
    protected BaseEntity()
    {
        Id = Guid.NewGuid();
        var now = DateTime.UtcNow;
        CreatedAt = now;
        UpdatedAt = now;
        IsActive = true;
    }
}
