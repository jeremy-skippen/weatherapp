using System.Security.Claims;
using System.Text.Encodings.Web;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace JeremySkippen.WeatherApp;

public sealed class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public IEnumerable<string> ApiKeys { get; set; }

    public ApiKeyAuthenticationOptions()
    {
        ApiKeys ??= [];
    }
};

public sealed class ApiKeyAuthenticationHandler(
    IOptionsMonitor<ApiKeyAuthenticationOptions> options,
    ILoggerFactory logger,
    UrlEncoder urlEncoder
) : AuthenticationHandler<ApiKeyAuthenticationOptions>(options, logger, urlEncoder)
{
    public const string AUTHENTICATION_SCHEME_NAME = "ApiKey";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var apiKey = Request.Headers.Authorization
            .Select(h => h?.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .Where(headerPair => headerPair is not null && headerPair.Length == 2 && headerPair[0].Equals(AUTHENTICATION_SCHEME_NAME, StringComparison.InvariantCultureIgnoreCase))
            .Select(headerPair => headerPair![1])
            .FirstOrDefault();

        if (apiKey is null)
            return Task.FromResult(AuthenticateResult.NoResult());

        var apiKeyIsValid = Options.ApiKeys.Any(Key => Key.Equals(apiKey, StringComparison.InvariantCulture));
        if (!apiKeyIsValid)
            return Task.FromResult(AuthenticateResult.Fail("Invalid API Key"));

        var ticket = new AuthenticationTicket(
            new ClaimsPrincipal(
                new ClaimsIdentity([new Claim(ClaimTypes.Name, apiKey)], AUTHENTICATION_SCHEME_NAME)
            ),
            AUTHENTICATION_SCHEME_NAME
        );

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
