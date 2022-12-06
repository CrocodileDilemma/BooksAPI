using BooksAPI.Interfaces;
using BooksAPI.Entities;
using Dapper;

namespace BooksAPI.Services;

public class BookService : IBookService
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public BookService(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<bool> CreateAsync(Book book)
    {
        var searchResult = await this.GetByIsbnAsync(book.Isbn);
        if (searchResult is not null)
        {
            return false;
        }

        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var result = await connection.ExecuteAsync(
            @"INSERT INTO Books (Isbn, Title, Author, ShortDescription, PageCount, ReleaseDate) 
            VALUES (@Isbn, @Title, @Author, @ShortDescription, @PageCount, @ReleaseDate)", 
            book);
        
        return result > 0;
    }

    public async Task<bool> DeleteAsync(string isbn)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var result = await connection.ExecuteAsync("DELETE FROM Books WHERE Isbn = @Isbn", new { Isbn = isbn });
        
        return result > 0;
    }

    public async Task<IEnumerable<Book>> GetAllAsync()
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        return await connection.QueryAsync<Book>("SELECT * FROM Books");
    }

    public async Task<Book?> GetByIsbnAsync(string isbn)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        return await connection.QuerySingleOrDefaultAsync<Book>("SELECT * FROM Books WHERE Isbn = @Isbn LIMIT 1", new { Isbn = isbn});
    }

    public async Task<IEnumerable<Book>> SearchByTitleAsync(string searchTerm)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        return await connection.QueryAsync<Book>("SELECT * FROM Books WHERE Title LIKE '%' || @SearchTerm || '%'", new { SearchTerm = searchTerm });
    }

    public async Task<bool> UpdateAsync(Book book)
    {
        var searchResult = await this.GetByIsbnAsync(book.Isbn);
        if (searchResult is null) 
        {
            return false;
        }

        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var result = await connection.ExecuteAsync(
            @"UPDATE Books SET Title = @Title, Author = @Author, ShortDescription = @ShortDescription, PageCount = @PageCount, ReleaseDate = @ReleaseDate 
            WHERE Isbn = @Isbn", book);

        return result > 0;

    }
}
