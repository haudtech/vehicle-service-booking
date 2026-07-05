using System.Security.Cryptography;
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

    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IRolePermissionRepository _rolePermissionRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
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
        JwtOptions jwtOptions)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _jwtOptions = jwtOptions;
    }

    /// <inheritdoc />
    public async Task<AuthResponse> SignUpAsync(SignUpRequest request, string ipAddress, CancellationToken cancellationToken = default)
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

        var user = new User
        {
            Email = normalizedEmail,
            AccountName = normalizedAccountName,
            DisplayName = request.DisplayName.Trim(),
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            SecurityStamp = Guid.NewGuid().ToString("N"),
            IsActive = true
        };

        await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

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

        return await CreateAuthResponseAsync(user, ipAddress, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<AuthResponse> LoginAsync(LoginRequest request, string ipAddress, CancellationToken cancellationToken = default)
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
