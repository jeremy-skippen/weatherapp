using System.Net;
using System.Text;
using Moq;
using Moq.Protected;

namespace JeremySkippen.WeatherApp.Tests;

public class OpenWeatherMapClientTests
{
    private readonly Mock<IHttpClientFactory> _mockHttpClientFactory = new();
    private readonly TaskCompletionSource<HttpResponseMessage> _httpResponseMessageSource = new();

    public OpenWeatherMapClientTests()
    {
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(_httpResponseMessageSource.Task)
            .Verifiable();

        _mockHttpClientFactory
            .Setup(f => f.CreateClient("OpenWeatherMapApi"))
            .Returns(new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://openweathermap"),
            });
    }

    [Fact]
    public async Task Verify_GetWeather_ThrowsWeatherNotFoundException_WhenOpenWeatherMapReturns404()
    {
        var sut = new OpenWeatherMapClient(_mockHttpClientFactory.Object);

        _httpResponseMessageSource.SetResult(new HttpResponseMessage(HttpStatusCode.NotFound));

        await Assert.ThrowsAsync<WeatherNotFoundException>(async () =>
        {
            await sut.GetWeather("city", "country", CancellationToken.None);
        });
    }

    [Fact]
    public async Task Verify_GetWeather_ReturnsWeatherDescription()
    {
        var sut = new OpenWeatherMapClient(_mockHttpClientFactory.Object);

        _httpResponseMessageSource.SetResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(@"{
    ""coord"": {
        ""lon"": -0.1257,
        ""lat"": 51.5085
    },
    ""weather"": [
        {
            ""id"": 803,
            ""main"": ""Clouds"",
            ""description"": ""broken clouds"",
            ""icon"": ""04d""
        }
    ],
    ""base"": ""stations"",
    ""main"": {
        ""temp"": 283.25,
        ""feels_like"": 282.64,
        ""temp_min"": 282.1,
        ""temp_max"": 284.26,
        ""pressure"": 1014,
        ""humidity"": 89
    },
    ""visibility"": 10000,
    ""wind"": {
        ""speed"": 1.54,
        ""deg"": 350
    },
    ""clouds"": {
        ""all"": 75
    },
    ""dt"": 1716184510,
    ""sys"": {
        ""type"": 2,
        ""id"": 2075535,
        ""country"": ""GB"",
        ""sunrise"": 1716177681,
        ""sunset"": 1716234783
    },
    ""timezone"": 3600,
    ""id"": 2643743,
    ""name"": ""London"",
    ""cod"": 200
}", Encoding.UTF8, "application/json")
        });

        var response = await sut.GetWeather("city", "country", CancellationToken.None);

        Assert.Equal("broken clouds", response);
    }
}
