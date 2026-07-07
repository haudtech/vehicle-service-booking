using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Moq.Protected;
using VehicleServiceBooking.Api.Configuration;
using Xunit;

namespace VehicleServiceBooking.Tests.Api.Configuration;

public class JwksSigningKeyProviderTests
{
    [Fact]
    public void GetSigningKeys_WhenKidExistsInCachedSet_ReturnsMatchingKeyWithoutExtraRefresh()
    {
        var factory = CreateHttpClientFactory(CreateHttpClient(BuildJwks("kid-a")));
        var sut = CreateProvider(factory.Object);

        var first = sut.GetSigningKeys("kid-a");
        var second = sut.GetSigningKeys("kid-a");

        first.Should().HaveCount(1);
        first.Single().KeyId.Should().Be("kid-a");
        second.Should().HaveCount(1);
        second.Single().KeyId.Should().Be("kid-a");

        factory.Verify(f => f.CreateClient("auth-jwks"), Times.Once);
    }

    [Fact]
    public void GetSigningKeys_WhenKidMissing_RefreshesOnceAndReturnsResolvedKey()
    {
        var client = CreateHttpClient(
            BuildJwks("kid-old"),
            BuildJwks("kid-old", "kid-new"));

        var factory = CreateHttpClientFactory(client);
        var sut = CreateProvider(factory.Object);

        // Prime cache from initial JWKS response.
        var seeded = sut.GetSigningKeys("kid-old");
        seeded.Should().HaveCount(1);

        var resolved = sut.GetSigningKeys("kid-new");

        resolved.Should().HaveCount(1);
        resolved.Single().KeyId.Should().Be("kid-new");

        // One initial load + one forced refresh on kid miss.
        factory.Verify(f => f.CreateClient("auth-jwks"), Times.Exactly(2));
    }

    [Fact]
    public void GetSigningKeys_WhenKidStillMissingAfterRefresh_ReturnsEmptySet()
    {
        var client = CreateHttpClient(
            BuildJwks("kid-a"),
            BuildJwks("kid-a"));

        var factory = CreateHttpClientFactory(client);
        var sut = CreateProvider(factory.Object);

        sut.GetSigningKeys("kid-a").Should().HaveCount(1);

        var unresolved = sut.GetSigningKeys("kid-missing");

        unresolved.Should().BeEmpty();
        factory.Verify(f => f.CreateClient("auth-jwks"), Times.Exactly(2));
    }

    private static JwksSigningKeyProvider CreateProvider(IHttpClientFactory factory)
    {
        var options = new AuthJwtOptions
        {
            Issuer = "http://localhost:5158",
            Audience = "vehicle-booking-api",
            JwksUrl = "http://auth.local/.well-known/jwks.json",
            JwksCacheMinutes = 15
        };

        return new JwksSigningKeyProvider(
            factory,
            options,
            Mock.Of<ILogger<JwksSigningKeyProvider>>());
    }

    private static Mock<IHttpClientFactory> CreateHttpClientFactory(HttpClient client)
    {
        var factory = new Mock<IHttpClientFactory>();
        factory.Setup(f => f.CreateClient("auth-jwks")).Returns(client);
        return factory;
    }

    private static HttpClient CreateHttpClient(params string[] jwksResponses)
    {
        var queue = new Queue<string>(jwksResponses);
        var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        handler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() =>
            {
                var payload = queue.Count > 0 ? queue.Dequeue() : jwksResponses[^1];
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(payload, Encoding.UTF8, "application/json")
                };
            });

        return new HttpClient(handler.Object);
    }

    private static string BuildJwks(params string[] kids)
    {
        var jsonWebKeys = kids.Select(kid =>
        {
            using var rsa = RSA.Create(2048);
            var rsaSecurityKey = new RsaSecurityKey(rsa.ExportParameters(includePrivateParameters: false))
            {
                KeyId = kid
            };
            var jsonWebKey = JsonWebKeyConverter.ConvertFromRSASecurityKey(rsaSecurityKey);
            jsonWebKey.Kid = kid;
            jsonWebKey.Use = "sig";
            jsonWebKey.Alg = SecurityAlgorithms.RsaSha256;
            return jsonWebKey;
        }).ToList();

        return JsonSerializer.Serialize(new { keys = jsonWebKeys });
    }
}
