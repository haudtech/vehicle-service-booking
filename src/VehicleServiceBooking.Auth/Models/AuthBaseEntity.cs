namespace VehicleServiceBooking.Auth.Models;

/// <summary>
/// Base class for auth entities with audit and activation fields.
/// </summary>
public abstract class AuthBaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsActive { get; set; }

    /// <summary>
    /// Initializes default values for new entities.
    /// </summary>
    protected AuthBaseEntity()
    {
        Id = Guid.NewGuid();
        var now = DateTime.UtcNow;
        CreatedAt = now;
        UpdatedAt = now;
        IsActive = true;
    }
}