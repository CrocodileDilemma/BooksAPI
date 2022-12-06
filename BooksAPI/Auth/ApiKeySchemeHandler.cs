using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace BooksAPI.Auth;

public class ApiKeySchemeHandler : AuthenticationHandler<ApiKeySchemeOptions>
{
    public ApiKeySchemeHandler(IOptionsMonitor<ApiKeySchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey(HeaderNames.Authorization))
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid API Key"));
        }

        var header = Request.Headers[HeaderNames.Authorization].ToString();
        if (!header.Equals(Options.ApiKey))
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid API Key"));
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.Email, "sam.moretta@gmail.com"),
            new Claim(ClaimTypes.Name, "sam.moretta")
        };

        var identity = new ClaimsIdentity(claims, "ApiKey");

        var ticket = new AuthenticationTicket(
            new ClaimsPrincipal(identity), Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
