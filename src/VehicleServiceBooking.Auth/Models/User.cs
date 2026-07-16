using VehicleServiceBooking.Auth.Common.Enums;

namespace VehicleServiceBooking.Auth.Models;

/// <summary>
/// Represents an authenticated system user.
/// </summary>
public class User : AuthBaseEntity
{
    public string Email { get; set; } = string.Empty;

    public bool IsEmailVerified { get; set; }

    public DateTime? EmailVerifiedAtUtc { get; set; }

    public string EmailVerificationTokenHash { get; set; } = string.Empty;

    public DateTime? EmailVerificationTokenExpiresAtUtc { get; set; }

    public Guid? LoginVerificationChallengeId { get; set; }

    public string LoginVerificationCodeHash { get; set; } = string.Empty;

    public DateTime? LoginVerificationCodeExpiresAtUtc { get; set; }

    public int LoginVerificationCodeAttempts { get; set; }

    public string LoginVerificationChannel { get; set; } = ChallengeChannel.EmailOtp.ToWireValue();

    public bool IsAuthenticatorAppEnabled { get; set; }

    public string AuthenticatorAppSecret { get; set; } = string.Empty;

    public string ZaloUserId { get; set; } = string.Empty;

    public string AccountName { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string SecurityStamp { get; set; } = Guid.NewGuid().ToString();

    public string DisplayName { get; set; } = string.Empty;

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
