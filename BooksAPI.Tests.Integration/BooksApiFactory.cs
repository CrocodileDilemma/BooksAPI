using BooksAPI.Data;
using BooksAPI.Interfaces;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BooksAPI.Tests.Integration
{
    public class BooksApiFactory : WebApplicationFactory<IApiMarker>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(collection =>
            {
                collection.RemoveAll(typeof(IDbConnectionFactory));
                collection.AddSingleton<IDbConnectionFactory>(_ =>
                    new SqliteConnectionFactory("DataSource=file:inmem?mode=memory&cache=shared"));
                });
        }
    }
}
