using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using VehicleServiceBooking.Auth.Configuration;
using VehicleServiceBooking.Auth.Controllers;
using VehicleServiceBooking.Auth.Models.Responses;
using VehicleServiceBooking.Auth.Services;

namespace VehicleServiceBooking.Tests.Auth.Controllers;

public class AuthControllerGoogleTests
{
    [Fact]
    public void StartGoogleLogin_WhenEnabled_ReturnsChallengeResultForGoogleScheme()
    {
        var authService = new Mock<IAuthService>();
        var sut = new AuthController(authService.Object, new GoogleAuthOptions { Enabled = true });
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
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, "google.user@example.com"),
            new(ClaimTypes.Name, "Google User"),
            new(ClaimTypes.NameIdentifier, "google-subject-123")
        };

        var sut = new AuthController(authService.Object, new GoogleAuthOptions { Enabled = true });
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

        var sut = new AuthController(authService.Object, new GoogleAuthOptions { Enabled = true });
        sut.ControllerContext = BuildControllerContext(CreateAuthenticationService(claims));

        var result = await sut.GoogleCallback(CancellationToken.None);

        result.Should().BeOfType<UnauthorizedObjectResult>();
        authService.Verify(x => x.LoginWithGoogleAsync(
            It.IsAny<string>(),
            It.IsAny<string?>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Never);
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