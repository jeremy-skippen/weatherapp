using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JeremySkippen.WeatherApp;

public interface IOpenWeatherMapClient
{
    Task<string> GetWeather(string cityName, string countryName, CancellationToken ct);
}

internal sealed class OpenWeatherMapClient(IHttpClientFactory httpClientFactory) : IOpenWeatherMapClient
{
    private static readonly string[] API_KEYS = [
        "8b7535b42fe1c551f18028f64e8688f7",
        "9f933451cebf1fa39de168a29a4d9a79",
    ];

    public async Task<string> GetWeather(string cityName, string countryName, CancellationToken ct)
    {
        var httpClient = httpClientFactory.CreateClient("OpenWeatherMapApi");
        var apiKey = API_KEYS[Random.Shared.Next(0, 1)];

        var requestUri = $"https://api.openweathermap.org/data/2.5/weather?q={Uri.EscapeDataString(cityName)},{Uri.EscapeDataString(countryName)}&appid={apiKey}";
        var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

        var response = await httpClient.SendAsync(request, ct);
        if (response.StatusCode == HttpStatusCode.NotFound)
            throw new WeatherNotFoundException();

        var responseBody = await response.Content.ReadAsStringAsync(ct);
        var typedResponse = JsonSerializer.Deserialize<WeatherResponse>(responseBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        });

        var weather = typedResponse?.Weather?.FirstOrDefault()?.Description;
        if (string.IsNullOrWhiteSpace(weather))
            throw new WeatherNotFoundException();

        return weather;
    }
}

internal sealed class WeatherResponse
{
    [JsonPropertyName("cod")]
    public int Code { get; set; }

    public string? Message { get; set; }

    public WeatherResponseWeatherItem[]? Weather { get; set; }
}

internal sealed class WeatherResponseWeatherItem
{
    public string Description { get; set; } = "";
}

internal sealed class WeatherNotFoundException : Exception
{
}
