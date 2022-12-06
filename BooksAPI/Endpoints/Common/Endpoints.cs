using BooksAPI.Interfaces;

namespace BooksAPI.Endpoints.Common;

public class Endpoints : IEndpoints
{
    public static string BaseRoute { get { return "common"; } }
    public static string TagName { get { return "Common"; } }
    public static void AddEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("status", CommonEndpoints.GetStatus)
            .ExcludeFromDescription();
    }

    public static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
    }
}
