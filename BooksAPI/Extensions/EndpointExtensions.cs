using BooksAPI.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace BooksAPI.Extensions;

public static class EndpointExtensions
{
    public static void AddEndpoints<TMarker>(this IServiceCollection services, IConfiguration configuration)
    {
        AddEndpoints(services, typeof(TMarker), configuration);
    }

    public static void AddEndpoints(this IServiceCollection services, Type typeMarker, IConfiguration configuration)
    {
        var endpointTypes = GetEnpointsFromAssemblyContaining(typeMarker);

        foreach (var endpointType in endpointTypes)
        {
            endpointType.GetMethod(nameof(IEndpoints.AddServices))!
                .Invoke(null, new object[] { services, configuration });
        }
    }

    public static void UseEndpoints<TMarker>(this IApplicationBuilder app)
    {
        UseEndpoints(app, typeof(TMarker));
    }

    public static void UseEndpoints(this IApplicationBuilder app, Type typeMarker)
    {
        var endpointTypes = GetEnpointsFromAssemblyContaining(typeMarker);

        foreach (var endpointType in endpointTypes)
        {
            endpointType.GetMethod(nameof(IEndpoints.AddEndpoints))!
                .Invoke(null, new object[] { app });
        }
    }

    private static IEnumerable<TypeInfo> GetEnpointsFromAssemblyContaining(Type typeMarker)
    {
        return typeMarker.Assembly.DefinedTypes.Where(x => !x.IsAbstract && !x.IsInterface &&
                    typeof(IEndpoints).IsAssignableFrom(x));
    }
}
