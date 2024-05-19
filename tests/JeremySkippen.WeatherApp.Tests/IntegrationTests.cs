using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;

namespace JeremySkippen.WeatherApp.Tests;

public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public IntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Verify_UnauthenticatedRequests_Return401()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/weather");

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

        var response = await client.GetAsync("/weather");

        response.EnsureSuccessStatusCode();
    }
}
