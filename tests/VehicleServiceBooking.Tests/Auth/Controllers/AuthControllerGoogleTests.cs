using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using VehicleServiceBooking.Auth.Configuration;
using VehicleServiceBooking.Auth.Controllers;
using VehicleServiceBooking.Auth.Models.Notifications;
using VehicleServiceBooking.Auth.Models.Requests;
using VehicleServiceBooking.Auth.Models.Responses;
using VehicleServiceBooking.Auth.Services;

namespace VehicleServiceBooking.Tests.Auth.Controllers;

public class AuthControllerGoogleTests
{
    [Fact]
    public void StartGoogleLogin_WhenEnabled_ReturnsChallengeResultForGoogleScheme()
    {
        var authService = new Mock<IAuthService>();
        var notificationPublisher = new Mock<INotificationPublisher>();
        var sut = new AuthController(
            authService.Object,
            new GoogleAuthOptions { Enabled = true },
            new NotificationOptions { Enabled = false },
            notificationPublisher.Object,
            NullLogger<AuthController>.Instance);
        sut.ControllerContext = BuildControllerContext();

        var result = sut.StartGoogleLogin();

        var challenge = result.Should().BeOfType<ChallengeResult>().Subject;
        challenge.AuthenticationSchemes.Should().ContainSingle(x => x == GoogleDefaults.AuthenticationScheme);
    }

    [Fact]
    public async Task GoogleCallback_WhenExternalIdentityValid_ReturnsAuthResponse()
    {
        var expected = new AuthResponse
        {
            AccessToken = "token",
            RefreshToken = "refresh",
            ExpiresAt = DateTime.UtcNow.AddDays(1)
        };

        var authService = new Mock<IAuthService>();
        authService
            .Setup(x => x.LoginWithGoogleAsync(
                "google.user@example.com",
                "Google User",
                "google-subject-123",
                true,
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, "google.user@example.com"),
            new(ClaimTypes.Name, "Google User"),
            new(ClaimTypes.NameIdentifier, "google-subject-123"),
            new("email_verified", "true")
        };

        var notificationPublisher = new Mock<INotificationPublisher>();
        var sut = new AuthController(
            authService.Object,
            new GoogleAuthOptions { Enabled = true },
            new NotificationOptions { Enabled = false },
            notificationPublisher.Object,
            NullLogger<AuthController>.Instance);
        sut.ControllerContext = BuildControllerContext(CreateAuthenticationService(claims));

        var result = await sut.GoogleCallback(CancellationToken.None);

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GoogleCallback_WhenEmailMissing_ReturnsUnauthorized()
    {
        var authService = new Mock<IAuthService>();

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, "google-subject-123")
        };

        var notificationPublisher = new Mock<INotificationPublisher>();
        var sut = new AuthController(
            authService.Object,
            new GoogleAuthOptions { Enabled = true },
            new NotificationOptions { Enabled = false },
            notificationPublisher.Object,
            NullLogger<AuthController>.Instance);
        sut.ControllerContext = BuildControllerContext(CreateAuthenticationService(claims));

        var result = await sut.GoogleCallback(CancellationToken.None);

        result.Should().BeOfType<UnauthorizedObjectResult>();
        authService.Verify(x => x.LoginWithGoogleAsync(
            It.IsAny<string>(),
            It.IsAny<string?>(),
            It.IsAny<string>(),
            It.IsAny<bool>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GoogleCallback_WhenServiceRejectsUnverifiedEmail_ReturnsUnauthorized()
    {
        var authService = new Mock<IAuthService>();
        authService
            .Setup(x => x.LoginWithGoogleAsync(
                It.IsAny<string>(),
                It.IsAny<string?>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Google identity email is not verified."));

        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, "google.user@example.com"),
            new(ClaimTypes.Name, "Google User"),
            new(ClaimTypes.NameIdentifier, "google-subject-123"),
            new("email_verified", "true")
        };

        var notificationPublisher = new Mock<INotificationPublisher>();
        var sut = new AuthController(
            authService.Object,
            new GoogleAuthOptions { Enabled = true },
            new NotificationOptions { Enabled = false },
            notificationPublisher.Object,
            NullLogger<AuthController>.Instance);
        sut.ControllerContext = BuildControllerContext(CreateAuthenticationService(claims));

        var result = await sut.GoogleCallback(CancellationToken.None);

        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task SignUp_WhenNotificationPublishTimesOut_ReturnsAcceptedResponse()
    {
        var expected = new SignUpResult
        {
            Email = "signup.user@example.com",
            VerificationToken = "verification-token",
            VerificationTokenExpiresAtUtc = DateTime.UtcNow.AddHours(24)
        };

        var authService = new Mock<IAuthService>();
        authService
            .Setup(x => x.SignUpAsync(It.IsAny<SignUpRequest>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var notificationPublisher = new Mock<INotificationPublisher>();
        notificationPublisher
            .Setup(x => x.PublishAsync(It.IsAny<NotificationMessage>(), It.IsAny<CancellationToken>()))
            .Returns<NotificationMessage, CancellationToken>((_, token) =>
            {
                token.IsCancellationRequested.Should().BeTrue();
                return Task.FromCanceled(token);
            });

        var sut = new AuthController(
            authService.Object,
            new GoogleAuthOptions { Enabled = false },
            new NotificationOptions { Enabled = true, PublishTimeout = TimeSpan.Zero },
            notificationPublisher.Object,
            NullLogger<AuthController>.Instance);
        sut.ControllerContext = BuildControllerContext();

        var urlHelper = new Mock<IUrlHelper>();
        urlHelper
            .SetupGet(x => x.ActionContext)
            .Returns(new ActionContext
            {
                HttpContext = new DefaultHttpContext(),
                RouteData = new RouteData(),
                ActionDescriptor = new ActionDescriptor()
            });
        urlHelper
            .Setup(x => x.Action(It.IsAny<UrlActionContext>()))
            .Returns("http://localhost:5158/api/v1/auth/verify-email?email=signup.user@example.com&token=verification-token");
        sut.Url = urlHelper.Object;

        var result = await sut.SignUp(new SignUpRequest
        {
            Email = "signup.user@example.com",
            AccountName = "signupuser",
            Password = "password123",
            DisplayName = "Signup User"
        }, CancellationToken.None);

        var accepted = result.Should().BeOfType<AcceptedResult>().Subject;
        accepted.Value.Should().NotBeNull();
    }

    private static ControllerContext BuildControllerContext(IAuthenticationService? authenticationService = null)
    {
        var services = new ServiceCollection();
        services.AddSingleton(authenticationService ?? CreateAuthenticationService(new List<Claim>()));

        var context = new DefaultHttpContext
        {
            RequestServices = services.BuildServiceProvider()
        };

        context.Request.Scheme = "http";
        context.Request.Host = new HostString("localhost", 5158);

        return new ControllerContext
        {
            HttpContext = context
        };
    }

    private static IAuthenticationService CreateAuthenticationService(IReadOnlyCollection<Claim> claims)
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, GoogleDefaults.AuthenticationScheme));
        var ticket = new AuthenticationTicket(principal, GoogleAuthOptions.ExternalCookieScheme);

        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService
            .Setup(x => x.AuthenticateAsync(It.IsAny<HttpContext>(), GoogleAuthOptions.ExternalCookieScheme))
            .ReturnsAsync(AuthenticateResult.Success(ticket));

        authenticationService
            .Setup(x => x.SignOutAsync(It.IsAny<HttpContext>(), GoogleAuthOptions.ExternalCookieScheme, It.IsAny<AuthenticationProperties>()))
            .Returns(Task.CompletedTask);

        authenticationService
            .Setup(x => x.ChallengeAsync(It.IsAny<HttpContext>(), It.IsAny<string?>(), It.IsAny<AuthenticationProperties?>()))
            .Returns(Task.CompletedTask);

        return authenticationService.Object;
    }
}