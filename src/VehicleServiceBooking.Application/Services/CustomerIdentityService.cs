using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VehicleServiceBooking.Application.Interfaces.Repositories;
using VehicleServiceBooking.Application.Interfaces.Services;
using VehicleServiceBooking.Domain.Entities;

namespace VehicleServiceBooking.Application.Services;

/// <summary>
/// Resolves and lazily creates booking customer records from auth user identity.
/// </summary>
public sealed class CustomerIdentityService : ICustomerIdentityService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IAuthUserProfileClient _authUserProfileClient;
    private readonly ILogger<CustomerIdentityService> _logger;

    public CustomerIdentityService(
        ICustomerRepository customerRepository,
        IAuthUserProfileClient authUserProfileClient,
        ILogger<CustomerIdentityService> logger)
    {
        _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
        _authUserProfileClient = authUserProfileClient ?? throw new ArgumentNullException(nameof(authUserProfileClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Customer> GetOrCreateCustomerByAuthUserIdAsync(Guid authUserId, CancellationToken cancellationToken = default)
    {
        var existing = await _customerRepository.GetByAuthUserIdAsync(authUserId, cancellationToken).ConfigureAwait(false);
        if (existing is not null)
        {
            return existing;
        }

        var profile = await _authUserProfileClient
            .GetUserCoreProfileByIdAsync(authUserId, cancellationToken)
            .ConfigureAwait(false);

        if (profile is null)
        {
            throw new InvalidOperationException("Auth user profile was not found.");
        }

        if (!profile.IsActive || !profile.IsEmailVerified)
        {
            throw new InvalidOperationException("Auth user is not eligible for customer bootstrap.");
        }

        var (firstName, lastName) = ResolveName(profile.DisplayName, profile.Email);

        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            AuthUserId = profile.AuthUserId,
            Email = profile.Email.Trim().ToLowerInvariant(),
            PhoneNumber = profile.PhoneNumber?.Trim() ?? string.Empty,
            FirstName = firstName,
            LastName = lastName,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActive = true
        };

        try
        {
            await _customerRepository.AddAsync(customer, cancellationToken).ConfigureAwait(false);
            return customer;
        }
        catch (DbUpdateException)
        {
            var concurrent = await _customerRepository.GetByAuthUserIdAsync(authUserId, cancellationToken).ConfigureAwait(false);
            if (concurrent is not null)
            {
                return concurrent;
            }

            throw;
        }
    }

    private (string FirstName, string LastName) ResolveName(string displayName, string email)
    {
        try
        {
            var normalizedDisplayName = (displayName ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(normalizedDisplayName))
            {
                var localPart = (email ?? string.Empty).Split('@', 2, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "user";
                var fallbackFirstName = Truncate(localPart, 100);
                return (fallbackFirstName, "User");
            }

            var segments = normalizedDisplayName
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (segments.Length == 1)
            {
                return (Truncate(segments[0], 100), "User");
            }

            var firstName = Truncate(segments[0], 100);
            var lastName = Truncate(string.Join(' ', segments.Skip(1)), 100);
            return (firstName, string.IsNullOrWhiteSpace(lastName) ? "User" : lastName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to resolve customer name from auth profile. Falling back to default names.");
            return ("User", "User");
        }
    }

    private static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
        {
            return value;
        }

        return value[..maxLength];
    }
}