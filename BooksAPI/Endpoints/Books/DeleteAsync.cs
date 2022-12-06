using BooksAPI.Interfaces;

namespace BooksAPI.Endpoints.Books;

internal partial class BookEndpoints
{
    internal static async Task<IResult> DeleteAsync(string isbn, IBookService service)
    {
        var result = await service.DeleteAsync(isbn);
        if (!result)
        {
            return Results.NotFound();
        }

        return Results.NoContent();
    }
}
