using BooksAPI.Interfaces;

namespace BooksAPI.Endpoints.Books;

internal partial class BookEndpoints
{
    internal static async Task<IResult> GetBooksAsync(IBookService service, string? searchTerm)
    {
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var searchResult = await service.SearchByTitleAsync(searchTerm);
            return Results.Ok(searchResult);
        }

        var result = await service.GetAllAsync();
        return Results.Ok(result);
    }
}
