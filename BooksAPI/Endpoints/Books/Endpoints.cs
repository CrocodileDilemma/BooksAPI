using BooksAPI.Entities;
using BooksAPI.Interfaces;
using BooksAPI.Services;
using FluentValidation;
using FluentValidation.Results;

namespace BooksAPI.Endpoints.Books;

public class Endpoints : IEndpoints
{
    public static string BaseRoute { get { return "books"; } }
    public static string TagName { get { return "Books"; } }

    public static void AddEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet(BaseRoute, BookEndpoints.GetBooksAsync)
            .WithName("GetBooks")
            .Produces<IEnumerable<Book>>(200)
            .WithTags(TagName);

        app.MapGet($"{BaseRoute}/{{isbn}}", BookEndpoints.GetAsync)
            .WithName("GetBook")
            .Produces<Book>(200)
            .Produces(404)
            .WithTags(TagName);

        app.MapPost(BaseRoute, BookEndpoints.PostAsync)
            .WithName("CreateBook")
            .Accepts<Book>("application/json")
            .Produces<Book>(201)
            .Produces<IEnumerable<ValidationFailure>>(400)
            .WithTags(TagName);

        app.MapPut($"{BaseRoute}/{{isbn}}", BookEndpoints.PutAsync)
            .WithName("UpdateBook")
            .Accepts<Book>("application/json")
            .Produces<Book>(200)
            .Produces<IEnumerable<ValidationFailure>>(400)
            .WithTags(TagName);

        app.MapDelete($"{BaseRoute}/{{isbn}}", BookEndpoints.DeleteAsync)
            .WithName("DeleteBook")
            .Produces(204)
            .Produces(404)
            .WithTags(TagName);
    }

    public static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IBookService, BookService>();
    }
}
