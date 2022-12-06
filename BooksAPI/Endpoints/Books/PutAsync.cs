using BooksAPI.Entities;
using BooksAPI.Interfaces;
using FluentValidation;

namespace BooksAPI.Endpoints.Books;

internal partial class BookEndpoints
{
    internal static async Task<IResult> PutAsync(string isbn, Book book, IBookService service, IValidator<Book> validator)
    {
        book.Isbn = isbn;

        var validationResult = await validator.ValidateAsync(book);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.Errors);
        }

        var result = await service.UpdateAsync(book);
        if (!result)
        {
            return Results.NotFound();
        }

        return Results.Ok(book);
    }
}
