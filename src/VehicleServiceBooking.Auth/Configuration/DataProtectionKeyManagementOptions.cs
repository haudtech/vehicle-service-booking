namespace VehicleServiceBooking.Auth.Configuration;

/// <summary>
/// Configurable options for Data Protection key-ring persistence and key lifetime.
/// </summary>
public sealed class DataProtectionKeyManagementOptions
{
    /// <summary>
    /// Gets or sets the logical application name used to isolate key rings.
    /// Instances that must decrypt each other's data must share this value.
    /// </summary>
    public string ApplicationName { get; set; } = "VehicleServiceBooking.Auth";

    /// <summary>
    /// Gets or sets the file-system directory for persisted key-ring files.
    /// In production this should point to durable shared storage across all instances.
    /// </summary>
    public string KeyRingPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets default key lifetime in days.
    /// </summary>
    public int DefaultKeyLifetimeDays { get; set; } = 90;
}
