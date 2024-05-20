using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace JeremySkippen.WeatherApp.Tests;

public class RateLimitingTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Mock<IOpenWeatherMapClient> _mockOpenWeatherMapClient = new();

    public RateLimitingTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(cfg =>
        {
            cfg.ConfigureTestServices(services =>
            {
                services.Replace(new ServiceDescriptor(typeof(IOpenWeatherMapClient), _mockOpenWeatherMapClient.Object));
            });
        });
    }

    [Fact]
    public async Task Verify_RateLimited_After5QuickRequests()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("ApiKey", "API-KEY-1");

        foreach (var _ in Enumerable.Range(1, 5))
        {
            var response = await client.GetAsync("/weather?cityName=city&countryName=country");
            response.EnsureSuccessStatusCode();
        }

        var throttledResponse = await client.GetAsync("/weather?cityName=city&countryName=country");
        Assert.Equal(HttpStatusCode.TooManyRequests, throttledResponse.StatusCode);
    }

    [Fact]
    public async Task Verify_RateLimiting_AppliedPerApiKey()
    {
        var client2 = _factory.CreateClient();
        client2.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("ApiKey", "API-KEY-2");

        var client3 = _factory.CreateClient();
        client3.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("ApiKey", "API-KEY-3");

        foreach (var _ in Enumerable.Range(1, 5))
        {
            var response = await client2.GetAsync("/weather?cityName=city&countryName=country");
            response.EnsureSuccessStatusCode();
        }

        var throttledResponse = await client2.GetAsync("/weather?cityName=city&countryName=country");
        Assert.Equal(HttpStatusCode.TooManyRequests, throttledResponse.StatusCode);

        var otherApiKeyResponse = await client3.GetAsync("/weather?cityName=city&countryName=country");
        otherApiKeyResponse.EnsureSuccessStatusCode();
    }
}
