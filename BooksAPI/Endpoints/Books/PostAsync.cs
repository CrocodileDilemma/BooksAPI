using BooksAPI.Entities;
using BooksAPI.Interfaces;
using FluentValidation;
using FluentValidation.Results;

namespace BooksAPI.Endpoints.Books;

internal partial class BookEndpoints
{
    internal static async Task<IResult> PostAsync(Book book, IBookService service, IValidator<Book> validator)
    {
        var validationResult = await validator.ValidateAsync(book);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.Errors);
        }

        var result = await service.CreateAsync(book);
        if (!result)
        {
            return Results.BadRequest(new List<ValidationFailure>
        {
                new ("Isbn", "A book with this ISBN-13 already exists!")
        });
        }

        return Results.CreatedAtRoute("GetBook", new { isbn = book.Isbn }, book);
    }
}
