using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace JeremySkippen.WeatherApp.Tests;

public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Mock<IOpenWeatherMapClient> _mockOpenWeatherMapClient = new();

    public IntegrationTests(WebApplicationFactory<Program> factory)
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
    public async Task Verify_UnauthenticatedRequests_Return401()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/weather?cityName=city&countryName=country");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [InlineData("API-KEY-1")]
    [InlineData("API-KEY-2")]
    [InlineData("API-KEY-3")]
    [InlineData("API-KEY-4")]
    [InlineData("API-KEY-5")]
    public async Task Verify_AuthenticatedRequests_Return200(string apiKey)
    {
        var client = _factory.CreateClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("ApiKey", apiKey);
        _mockOpenWeatherMapClient
            .Setup(c => c.GetWeather(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("response");

        var response = await client.GetAsync("/weather?cityName=city&countryName=country");

        response.EnsureSuccessStatusCode();

        _mockOpenWeatherMapClient.Verify(c => c.GetWeather("city", "country", It.IsAny<CancellationToken>()), Times.Once);
    }
}
