using BookAPI.Data;
using BooksAPI.Auth;
using BooksAPI.Data;
using BooksAPI.Extensions;
using BooksAPI.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Http.Json;

namespace BooksAPI;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            //WebRootPath = "./wwwroot",
            //EnvironmentName = Environment.GetEnvironmentVariable("env"),
            //ApplicationName = "Library.API,"
            Args = args
        });

        builder.Services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.PropertyNameCaseInsensitive = true;
            options.SerializerOptions.IncludeFields = true;
        });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AnyOrigin", x => x.AllowAnyOrigin());
        });

        builder.Configuration.AddJsonFile("appsettings.Local.json", true, true);
        
        builder.Services.AddAuthentication(ApiKeySchemeConstants.SchemeName)
            .AddScheme<ApiKeySchemeOptions, ApiKeySchemeHandler>(ApiKeySchemeConstants.SchemeName, _ => { });
        builder.Services.AddAuthorization();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddSingleton<IDbConnectionFactory>(_ =>
        new SqliteConnectionFactory(
            builder.Configuration.GetValue<string>("Database:ConnectionString")
            ));
        builder.Services.AddEndpoints<Program>(builder.Configuration);
        builder.Services.AddSingleton<DatabaseInitializer>();
        builder.Services.AddValidatorsFromAssemblyContaining<Program>();

        var app = builder.Build();

        app.UseCors();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseAuthorization();
        app.UseEndpoints<Program>();
        
        var dbInit = app.Services.GetRequiredService<DatabaseInitializer>();
        await dbInit.InitializeAsync();

        app.Run();
    }
}