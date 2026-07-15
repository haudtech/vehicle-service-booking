using ChallengeChannelEnum = VehicleServiceBooking.Auth.Common.Enums.ChallengeChannel;

namespace VehicleServiceBooking.Auth.Models.Responses;

/// <summary>
/// Represents a pending login challenge that requires user code verification.
/// </summary>
public sealed class LoginChallengeResult
{
    public string Email { get; set; } = string.Empty;

    public Guid ChallengeId { get; set; }

    public ChallengeChannelEnum ChallengeChannel { get; set; } = ChallengeChannelEnum.EmailOtp;

    public string VerificationCode { get; set; } = string.Empty;

    public DateTime VerificationCodeExpiresAtUtc { get; set; }
}
