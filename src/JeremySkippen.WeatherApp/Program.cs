using System.Threading.RateLimiting;
using JeremySkippen.WeatherApp;
using Microsoft.AspNetCore.Authentication;

const string DEV_CORS_POLICY_NAME = "DevPolicy";
const string RATE_LIMIT_POLICY_NAME = "ApiKeyRateLimiting";

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddEnvironmentVariables();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(opt =>
    {
        opt.AddPolicy(DEV_CORS_POLICY_NAME, builder =>
        {
            builder
                .WithOrigins("http://localhost:5173")
                .WithMethods("GET")
                .AllowAnyHeader();
        });
    });
}

builder.Services
    .AddLogging()
    .AddHttpClient()
    .AddScoped<IOpenWeatherMapClient, OpenWeatherMapClient>()
    .AddProblemDetails()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

builder.Services
    .AddAuthorization()
    .AddAuthentication(ApiKeyAuthenticationHandler.AUTHENTICATION_SCHEME_NAME)
    .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(ApiKeyAuthenticationHandler.AUTHENTICATION_SCHEME_NAME, options =>
    {
        // TODO: Configure this better
        options.ApiKeys = [
            "API-KEY-1",
            "API-KEY-2",
            "API-KEY-3",
            "API-KEY-4",
            "API-KEY-5",
        ];
    });

builder.Services
    .AddRateLimiter(opt =>
    {
        opt.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

        opt.AddPolicy(RATE_LIMIT_POLICY_NAME, httpContext =>
        {
            FixedWindowRateLimiterOptions rateLimitOptions = new()
            {
                // TODO: Make this configurable?
                Window = TimeSpan.FromHours(1),
                PermitLimit = 5,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0,
                AutoReplenishment = true,
            };

            var authenticateResult = httpContext.Features.Get<IAuthenticateResultFeature>()?.AuthenticateResult;
            var identityName = authenticateResult?.Principal?.Identity?.Name;
            if (!string.IsNullOrWhiteSpace(identityName))
                return RateLimitPartition.GetFixedWindowLimiter(identityName, _ => rateLimitOptions);

            return RateLimitPartition.GetFixedWindowLimiter("unauthenticated", _ => rateLimitOptions);
        });
    });

builder.Logging
    .ClearProviders()
    .AddConsole();

var app = builder.Build();

app.UseAuthorization();
app.UseRateLimiter();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(DEV_CORS_POLICY_NAME);
}

app.MapGet("/weather", async (string cityName, string countryName, IOpenWeatherMapClient weatherClient, CancellationToken ct) =>
    {
        try
        {
            return Results.Ok(await weatherClient.GetWeather(cityName, countryName, ct));
        }
        catch (WeatherNotFoundException)
        {
            return Results.NotFound();
        }
    })
    .WithName("GetWeather")
    .WithOpenApi()
    .RequireAuthorization()
    .RequireRateLimiting(RATE_LIMIT_POLICY_NAME);

app.Run();

public partial class Program { }
