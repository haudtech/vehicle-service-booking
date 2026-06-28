namespace VehicleServiceBooking.Application.Configuration;

/// <summary>
/// Configuration for caching static tables/entities only.
/// This configuration must not be used for mutable transactional entities.
/// </summary>
public class StaticDataCacheOptions
{
    public const string SectionName = "StaticDataCache";

    /// <summary>
    /// Global toggle for static-entity caching only.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Enables caching for TimeSlot static data.
    /// </summary>
    public bool CacheTimeSlots { get; set; } = true;

    /// <summary>
    /// Enables caching for AppointmentStatusLookup static data.
    /// </summary>
    public bool CacheAppointmentStatuses { get; set; } = true;

    /// <summary>
    /// Enables caching for ServiceStatusLookup static data.
    /// </summary>
    public bool CacheServiceStatuses { get; set; } = true;

    public int TimeSlotsTtlMinutes { get; set; } = 1440;

    public int AppointmentStatusesTtlMinutes { get; set; } = 1440;

    public int ServiceStatusesTtlMinutes { get; set; } = 1440;
}
