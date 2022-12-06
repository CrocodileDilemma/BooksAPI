namespace BooksAPI.Interfaces;

public interface IEndpoints
{
    public static string BaseRoute { get; }
    public static string TagName { get; }
    public static abstract void AddEndpoints(IEndpointRouteBuilder app);
    public static abstract void AddServices(IServiceCollection services, IConfiguration configuration);
}
