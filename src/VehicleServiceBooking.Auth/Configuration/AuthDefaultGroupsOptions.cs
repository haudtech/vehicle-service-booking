namespace VehicleServiceBooking.Auth.Configuration;

/// <summary>
/// Configurable defaults for auth role assignment.
/// </summary>
public sealed class AuthDefaultGroupsOptions
{
    public const string SectionName = "AuthDefaultGroups";

    public List<string> DefaultSignupGroupNames { get; set; } = new();
}
