using Microsoft.AspNetCore.Authentication;

namespace BooksAPI.Auth;

public class ApiKeySchemeOptions : AuthenticationSchemeOptions
{
    public string ApiKey { get; set; } = "SecretKey";
}
