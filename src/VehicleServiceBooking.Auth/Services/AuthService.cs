using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using QRCoder;
using VehicleServiceBooking.Auth.Common.Enums;
using VehicleServiceBooking.Auth.Common.Verification;
using VehicleServiceBooking.Auth.Configuration;
using VehicleServiceBooking.Auth.Models;
using VehicleServiceBooking.Auth.Models.Requests;
using VehicleServiceBooking.Auth.Models.Responses;
using VehicleServiceBooking.Auth.Repositories.Interfaces;

namespace VehicleServiceBooking.Auth.Services;

/// <summary>
/// Implements authentication flows including sign-up, login, token refresh, and revocation.
/// </summary>
public sealed class AuthService : IAuthService
{
    private const string DefaultSignupRoleName = "booking-user";
    private static readonly TimeSpan EmailVerificationTokenLifetime = TimeSpan.FromHours(24);
    private static readonly TimeSpan LoginVerificationCodeLifetime = TimeSpan.FromMinutes(10);
    private const int LoginVerificationCodeMaxAttempts = 5;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IRolePermissionRepository _rolePermissionRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IAuthenticatorSecretProtector _authenticatorSecretProtector;
    private readonly JwtOptions _jwtOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthService"/> class.
    /// </summary>
    public AuthService(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository,
        IRolePermissionRepository rolePermissionRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IAuthenticatorSecretProtector authenticatorSecretProtector,
        JwtOptions jwtOptions)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _authenticatorSecretProtector = authenticatorSecretProtector;
        _jwtOptions = jwtOptions;
    }

    /// <inheritdoc />
    public async Task<SignUpResult> SignUpAsync(SignUpRequest request, string ipAddress, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var normalizedAccountName = request.AccountName.Trim().ToLowerInvariant();

        var existing = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);
        if (existing is not null)
        {
            throw new InvalidOperationException("A user with that email address already exists.");
        }

        var existingByAccountName = await _userRepository.GetByAccountNameAsync(normalizedAccountName, cancellationToken);
        if (existingByAccountName is not null)
        {
            throw new InvalidOperationException("A user with that account name already exists.");
        }

        var verificationToken = VerificationUtils.GenerateVerificationToken();
        var normalizedPhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber)
            ? string.Empty
            : request.PhoneNumber.Trim();

        var user = new User
        {
            Email = normalizedEmail,
            AccountName = normalizedAccountName,
            DisplayName = request.DisplayName.Trim(),
            PhoneNumber = normalizedPhoneNumber,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            SecurityStamp = Guid.NewGuid().ToString("N"),
            IsActive = true,
            IsEmailVerified = false,
            EmailVerificationTokenHash = VerificationUtils.HashVerificationToken(verificationToken),
            EmailVerificationTokenExpiresAtUtc = DateTime.UtcNow.Add(EmailVerificationTokenLifetime)
        };

        try
        {
            await _userRepository.AddAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            throw new InvalidOperationException("A user with that email address or account name already exists.");
        }

        await EnsureDefaultRoleAssignedAsync(user, cancellationToken);

        return new SignUpResult
        {
            Email = user.Email,
            VerificationToken = verificationToken,
            VerificationTokenExpiresAtUtc = user.EmailVerificationTokenExpiresAtUtc!.Value
        };
    }

    /// <inheritdoc />
    public async Task VerifyEmailAsync(VerifyEmailRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);
        if (user is null)
        {
            throw new InvalidOperationException("Invalid email verification request.");
        }

        if (user.IsEmailVerified)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(user.EmailVerificationTokenHash) ||
            !user.EmailVerificationTokenExpiresAtUtc.HasValue ||
            user.EmailVerificationTokenExpiresAtUtc.Value < DateTime.UtcNow)
        {
            throw new InvalidOperationException("Email verification token is invalid or expired.");
        }

        var providedHash = VerificationUtils.HashVerificationToken(request.Token.Trim());
        if (!CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(user.EmailVerificationTokenHash),
                Encoding.UTF8.GetBytes(providedHash)))
        {
            throw new InvalidOperationException("Email verification token is invalid or expired.");
        }

        user.IsEmailVerified = true;
        user.EmailVerifiedAtUtc = DateTime.UtcNow;
        user.EmailVerificationTokenHash = string.Empty;
        user.EmailVerificationTokenExpiresAtUtc = null;

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<LoginChallengeResult> StartLoginChallengeAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var rawIdentifier = string.IsNullOrWhiteSpace(request.Identifier)
            ? request.Email
            : request.Identifier;
        var normalizedIdentifier = rawIdentifier.Trim().ToLowerInvariant();
        var user = await _userRepository.GetActiveByEmailOrAccountNameWithAuthorizationAsync(normalizedIdentifier, cancellationToken);

        if (user is null || !_passwordHasher.VerifyPassword(user.PasswordHash, request.Password))
        {
            throw new InvalidOperationException("Invalid email or password.");
        }

        if (!user.IsEmailVerified)
        {
            throw new InvalidOperationException("Email is not verified.");
        }

        var requestedChannel = ChallengeChannelExtensions.ParseWireValueOrDefault(request.ChallengeChannel, ChallengeChannel.OtpFirst);
        var resolvedChannel = ResolveChallengeChannel(user, requestedChannel);

        var challengeId = Guid.NewGuid();
        user.LoginVerificationChallengeId = challengeId;
        user.LoginVerificationCodeExpiresAtUtc = DateTime.UtcNow.Add(LoginVerificationCodeLifetime);
        user.LoginVerificationCodeAttempts = 0;
        user.LoginVerificationChannel = resolvedChannel.ToWireValue();

        var code = string.Empty;
        if (resolvedChannel == ChallengeChannel.EmailOtp || resolvedChannel == ChallengeChannel.ZaloOtp)
        {
            code = VerificationUtils.GenerateLoginVerificationCode();
            user.LoginVerificationCodeHash = VerificationUtils.HashVerificationToken(code);
        }
        else
        {
            user.LoginVerificationCodeHash = string.Empty;
        }

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return new LoginChallengeResult
        {
            Email = user.Email,
            ChallengeId = challengeId,
            ChallengeChannel = resolvedChannel,
            VerificationCode = code,
            ZaloUserId = user.ZaloUserId,
            VerificationCodeExpiresAtUtc = user.LoginVerificationCodeExpiresAtUtc.Value
        };
    }

    /// <inheritdoc />
    public async Task<AuthenticatorSetupStartResult> StartAuthenticatorSetupAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null || !user.IsActive)
        {
            throw new InvalidOperationException("User account is not active.");
        }

        if (!user.IsEmailVerified)
        {
            throw new InvalidOperationException("Email must be verified before enabling authenticator app.");
        }

        var secret = AuthenticatorSetupUtils.GenerateBase32Secret();
        user.AuthenticatorAppSecret = _authenticatorSecretProtector.Protect(secret);
        user.IsAuthenticatorAppEnabled = false;

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        var otpauthUri = AuthenticatorSetupUtils.BuildOtpAuthUri(
            issuer: _jwtOptions.Issuer,
            accountName: user.Email,
            base32Secret: secret);

        return new AuthenticatorSetupStartResult
        {
            OtpauthUri = otpauthUri,
            QrPayload = otpauthUri,
            SecretMasked = AuthenticatorSetupUtils.MaskSecret(secret)
        };
    }

    /// <inheritdoc />
    public async Task<AuthenticatorSetupVerifyResult> VerifyAuthenticatorSetupAsync(Guid userId, AuthenticatorSetupVerifyRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null || !user.IsActive)
        {
            throw new InvalidOperationException("User account is not active.");
        }

        if (string.IsNullOrWhiteSpace(user.AuthenticatorAppSecret))
        {
            throw new InvalidOperationException("Authenticator setup has not been started.");
        }

        if (!_authenticatorSecretProtector.TryUnprotect(user.AuthenticatorAppSecret, out var plaintextSecret))
        {
            throw new InvalidOperationException("Authenticator setup secret is invalid. Please restart setup.");
        }

        if (!TotpVerificationUtils.VerifyCode(plaintextSecret, request.Code))
        {
            throw new InvalidOperationException("Authenticator code is invalid or expired.");
        }

        user.IsAuthenticatorAppEnabled = true;
        user.LoginVerificationChannel = ChallengeChannel.AuthenticatorApp.ToWireValue();

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return new AuthenticatorSetupVerifyResult
        {
            IsAuthenticatorAppEnabled = user.IsAuthenticatorAppEnabled,
            LoginVerificationChannel = user.LoginVerificationChannel
        };
    }

    /// <inheritdoc />
    public async Task<byte[]> GetAuthenticatorSetupQrCodePngAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null || !user.IsActive)
        {
            throw new InvalidOperationException("User account is not active.");
        }

        if (string.IsNullOrWhiteSpace(user.AuthenticatorAppSecret))
        {
            throw new InvalidOperationException("Authenticator setup has not been started.");
        }

        if (!_authenticatorSecretProtector.TryUnprotect(user.AuthenticatorAppSecret, out var plaintextSecret))
        {
            throw new InvalidOperationException("Authenticator setup secret is invalid. Please restart setup.");
        }

        var otpauthUri = AuthenticatorSetupUtils.BuildOtpAuthUri(
            issuer: _jwtOptions.Issuer,
            accountName: user.Email,
            base32Secret: plaintextSecret);

        using var generator = new QRCodeGenerator();
        using var qrData = generator.CreateQrCode(otpauthUri, QRCodeGenerator.ECCLevel.Q);
        var qrCode = new PngByteQRCode(qrData);
        return qrCode.GetGraphic(20);
    }

    /// <inheritdoc />
    public async Task<AuthResponse> VerifyLoginCodeAsync(LoginVerifyCodeRequest request, string ipAddress, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await _userRepository.GetActiveByEmailOrAccountNameWithAuthorizationAsync(normalizedEmail, cancellationToken);

        if (user is null || !user.IsEmailVerified)
        {
            throw new InvalidOperationException("Invalid login verification request.");
        }

        if (!user.LoginVerificationChallengeId.HasValue || user.LoginVerificationChallengeId.Value != request.ChallengeId)
        {
            throw new InvalidOperationException("Invalid login verification request.");
        }

        if (!user.LoginVerificationCodeExpiresAtUtc.HasValue || user.LoginVerificationCodeExpiresAtUtc.Value < DateTime.UtcNow)
        {
            throw new InvalidOperationException("Login verification code is invalid or expired.");
        }

        if (user.LoginVerificationCodeAttempts >= LoginVerificationCodeMaxAttempts)
        {
            throw new InvalidOperationException("Login verification attempts exceeded. Please restart login.");
        }

        var challengeChannel = ChallengeChannelExtensions.ParseWireValueOrDefault(
            user.LoginVerificationChannel,
            ChallengeChannel.EmailOtp);

        var isCodeValid = challengeChannel switch
        {
            ChallengeChannel.AuthenticatorApp => IsAuthenticatorCodeValid(user, request.Code),
            _ => IsEmailChallengeCodeValid(user, request.Code)
        };

        if (!isCodeValid)
        {
            user.LoginVerificationCodeAttempts += 1;
            await _userRepository.UpdateAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);
            throw new InvalidOperationException("Login verification code is invalid or expired.");
        }

        user.LoginVerificationChallengeId = null;
        user.LoginVerificationCodeHash = string.Empty;
        user.LoginVerificationCodeExpiresAtUtc = null;
        user.LoginVerificationCodeAttempts = 0;

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return await CreateAuthResponseAsync(user, ipAddress, cancellationToken);
    }

    private static ChallengeChannel ResolveChallengeChannel(User user, ChallengeChannel requestedChannel)
    {
        if (requestedChannel == ChallengeChannel.AuthenticatorApp)
        {
            if (!user.IsAuthenticatorAppEnabled || string.IsNullOrWhiteSpace(user.AuthenticatorAppSecret))
            {
                throw new InvalidOperationException("Authenticator app is not enabled for this account.");
            }

            return ChallengeChannel.AuthenticatorApp;
        }

        if (requestedChannel == ChallengeChannel.ZaloOtp)
        {
            // TODO: Once Zalo OA integration is ready, return ZaloOtp here and deliver the OTP through the Zalo notification pipeline.
            // For now, always fall back to EmailOtp until the Zalo sender is ready for production use.
            if (string.IsNullOrWhiteSpace(user.ZaloUserId))
            {
                return ChallengeChannel.EmailOtp;
            }

            return ChallengeChannel.EmailOtp;
        }

        if (requestedChannel == ChallengeChannel.OtpFirst &&
            user.IsAuthenticatorAppEnabled &&
            !string.IsNullOrWhiteSpace(user.AuthenticatorAppSecret))
        {
            return ChallengeChannel.AuthenticatorApp;
        }

        if (requestedChannel == ChallengeChannel.OtpFirst &&
            !string.IsNullOrWhiteSpace(user.ZaloUserId))
        {
            return ChallengeChannel.ZaloOtp;
        }

        return ChallengeChannel.EmailOtp;
    }

    private static bool IsEmailChallengeCodeValid(User user, string rawCode)
    {
        var providedHash = VerificationUtils.HashVerificationToken(rawCode.Trim());
        return !string.IsNullOrWhiteSpace(user.LoginVerificationCodeHash)
               && CryptographicOperations.FixedTimeEquals(
                   Encoding.UTF8.GetBytes(user.LoginVerificationCodeHash),
                   Encoding.UTF8.GetBytes(providedHash));
    }

    private bool IsAuthenticatorCodeValid(User user, string rawCode)
    {
        if (!user.IsAuthenticatorAppEnabled || string.IsNullOrWhiteSpace(user.AuthenticatorAppSecret))
        {
            return false;
        }

        return _authenticatorSecretProtector.TryUnprotect(user.AuthenticatorAppSecret, out var plaintextSecret)
               && TotpVerificationUtils.VerifyCode(plaintextSecret, rawCode);
    }

    /// <inheritdoc />
    public async Task<AuthResponse> LoginWithGoogleAsync(string email, string? displayName, string providerUserId, bool emailVerified, string ipAddress, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(providerUserId))
        {
            throw new InvalidOperationException("Google identity is missing provider subject.");
        }

        if (!emailVerified)
        {
            throw new InvalidOperationException("Google identity email is not verified.");
        }

        var normalizedEmail = email.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(normalizedEmail))
        {
            throw new InvalidOperationException("Google identity did not provide a usable email address.");
        }

        var user = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);
        if (user is null)
        {
            var normalizedAccountName = await GenerateUniqueAccountNameAsync(normalizedEmail, cancellationToken);
            var resolvedDisplayName = string.IsNullOrWhiteSpace(displayName)
                ? normalizedAccountName
                : displayName.Trim();

            user = new User
            {
                Email = normalizedEmail,
                AccountName = normalizedAccountName,
                DisplayName = resolvedDisplayName,
                PhoneNumber = string.Empty,
                PasswordHash = _passwordHasher.HashPassword(Convert.ToBase64String(RandomNumberGenerator.GetBytes(48))),
                SecurityStamp = Guid.NewGuid().ToString("N"),
                IsActive = true,
                IsEmailVerified = true,
                EmailVerifiedAtUtc = DateTime.UtcNow,
                EmailVerificationTokenHash = string.Empty,
                EmailVerificationTokenExpiresAtUtc = null
            };

            await _userRepository.AddAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);

            await EnsureDefaultRoleAssignedAsync(user, cancellationToken);
        }

        if (!user.IsActive)
        {
            throw new InvalidOperationException("User account is not active.");
        }

        return await CreateAuthResponseAsync(user, ipAddress, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, string ipAddress, CancellationToken cancellationToken = default)
    {
        var refreshToken = await _refreshTokenRepository.GetActiveByTokenWithUserAsync(request.RefreshToken, cancellationToken);

        if (refreshToken is null || !refreshToken.IsUsable)
        {
            throw new InvalidOperationException("Invalid or expired refresh token.");
        }

        var user = refreshToken.User;
        if (!user.IsActive)
        {
            throw new InvalidOperationException("User account is not active.");
        }

        refreshToken.RevokedAt = DateTime.UtcNow;
        await _refreshTokenRepository.UpdateAsync(refreshToken, cancellationToken);
        await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

        return await CreateAuthResponseAsync(user, ipAddress, cancellationToken);
    }

    /// <inheritdoc />
    public async Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken, cancellationToken);

        if (storedToken is null)
        {
            return;
        }

        storedToken.RevokedAt = DateTime.UtcNow;
        await _refreshTokenRepository.UpdateAsync(storedToken, cancellationToken);
        await _refreshTokenRepository.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<UserCoreProfileResponse?> GetUserCoreProfileByIdAsync(Guid authUserId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(authUserId, cancellationToken);
        if (user is null)
        {
            return null;
        }

        return new UserCoreProfileResponse
        {
            AuthUserId = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName,
            PhoneNumber = user.PhoneNumber,
            IsEmailVerified = user.IsEmailVerified,
            IsActive = user.IsActive,
            UpdatedAtUtc = user.UpdatedAt
        };
    }

    private async Task<AuthResponse> CreateAuthResponseAsync(User user, string ipAddress, CancellationToken cancellationToken)
    {
        var roles = await _userRoleRepository.GetRoleNamesByUserIdAsync(user.Id, cancellationToken);
        var permissions = await _rolePermissionRepository.GetPermissionNamesByUserIdAsync(user.Id, cancellationToken);

        var token = _jwtTokenGenerator.GenerateAccessToken(user, roles, permissions);
        var refreshToken = CreateRefreshToken(user.Id, ipAddress);

        await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
        await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

        return new AuthResponse
        {
            AccessToken = token,
            RefreshToken = refreshToken.Token,
            ExpiresAt = refreshToken.ExpiresAt
        };
    }

    private async Task EnsureDefaultRoleAssignedAsync(User user, CancellationToken cancellationToken)
    {
        var roleId = await _roleRepository.GetRoleIdByNameAsync(DefaultSignupRoleName, cancellationToken);
        if (roleId is null)
        {
            throw new InvalidOperationException($"Role '{DefaultSignupRoleName}' is not configured.");
        }

        var hasRole = await _userRoleRepository.ExistsAsync(user.Id, roleId.Value, cancellationToken);
        if (!hasRole)
        {
            await _userRoleRepository.AddAsync(new UserRole
            {
                UserId = user.Id,
                RoleId = roleId.Value
            }, cancellationToken);

            await _userRoleRepository.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task<string> GenerateUniqueAccountNameAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        var basePart = normalizedEmail.Split('@')[0];
        var sanitized = new string(basePart
            .Where(ch => char.IsLetterOrDigit(ch) || ch == '_' || ch == '.')
            .ToArray())
            .Trim();

        if (string.IsNullOrWhiteSpace(sanitized))
        {
            sanitized = "googleuser";
        }

        sanitized = sanitized[..Math.Min(40, sanitized.Length)];

        var candidate = sanitized;
        var suffix = 0;
        while (await _userRepository.GetByAccountNameAsync(candidate, cancellationToken) is not null)
        {
            suffix++;
            var suffixText = $"_{suffix}";
            var prefixLength = Math.Min(40, Math.Max(1, sanitized.Length));
            prefixLength = Math.Min(prefixLength, 50 - suffixText.Length);
            candidate = sanitized[..prefixLength] + suffixText;
        }

        return candidate;
    }

    private RefreshToken CreateRefreshToken(Guid userId, string ipAddress)
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var expiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenLifetimeDays);

        return new RefreshToken
        {
            Token = token,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = expiresAt,
            UserId = userId,
            CreatedByIp = ipAddress
        };
    }

}
