using BooksAPI.Interfaces;

namespace BooksAPI.Endpoints.Books;

internal partial class BookEndpoints
{
    internal static async Task<IResult> GetAsync(string isbn, IBookService service)
    {
        var result = await service.GetByIsbnAsync(isbn);
        if (result is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(result);
    }
}
