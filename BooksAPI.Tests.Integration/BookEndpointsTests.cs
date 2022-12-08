using BooksAPI.Entities;
using BooksAPI.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace BooksAPI.Tests.Integration;

public class BookEndpointsTests : IClassFixture<BooksApiFactory>, IAsyncLifetime
{
    private readonly BooksApiFactory _factory;
    private readonly List<string> _createdIsbns = new();

    public BookEndpointsTests(BooksApiFactory factory)
    {
        _factory = factory;
    }

    #region POST
    [Fact]
    public async Task CreateBook_CreatesBook_WhenDataIsValid()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        var book = GenerateBook();

        // Act
        var response = await httpClient.PostAsJsonAsync("/books", book);
        _createdIsbns.Add(book.Isbn);
        var result = await response.Content.ReadFromJsonAsync<Book>();

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        result.Should().BeEquivalentTo(book);
        response.Headers.Location.AbsolutePath.Should().Be($"/books/{book.Isbn}");
    }

    [Fact]
    public async Task CreateBook_Fails_WhenIsbnIsInvalid()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        var book = GenerateBook();
        book.Isbn = "INVALID";

        // Act
        var response = await httpClient.PostAsJsonAsync("/books", book);
        var errors = await response.Content.ReadFromJsonAsync<IEnumerable<ValidationError>>();
        var error = errors!.Single();

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        error.PropertyName.Should().Be("Isbn");
        error.ErrorMessage.Should().Be("Value was not a valid ISBN-13!");
    }

    [Fact]
    public async Task CreateBook_Fails_WhenBookExists()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        var book = GenerateBook();

        // Act
        await httpClient.PostAsJsonAsync("/books", book);
        _createdIsbns.Add(book.Isbn);
        var response = await httpClient.PostAsJsonAsync("/books", book);
        var errors = await response.Content.ReadFromJsonAsync<IEnumerable<ValidationError>>();
        var error = errors!.Single();

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        error.PropertyName.Should().Be("Isbn");
        error.ErrorMessage.Should().Be("A book with this ISBN-13 already exists!");
    }
    #endregion

    #region GET
    [Fact]
    public async Task GetBook_ReturnsBook_WhenBookExists()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        var book = GenerateBook();
        await httpClient.PostAsJsonAsync("/books", book);
        _createdIsbns.Add(book.Isbn);

        // Act
        var response = await httpClient.GetAsync($"/books/{book.Isbn}");    
        var result = await response.Content.ReadFromJsonAsync<Book>();

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        result.Should().BeEquivalentTo(book);
    }

    [Fact]
    public async Task GetBook_ReturnsNotFound_WhenBookDoesNotExist()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        var isbn = GenerateIsbn();

        // Act
        var response = await httpClient.GetAsync($"/books/{isbn}");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }
    #endregion

    #region GET LIST
    [Fact]
    public async Task GetBooks_ReturnsAllBooks_WhenBooksExists()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        var book = GenerateBook();
        await httpClient.PostAsJsonAsync("/books", book);
        _createdIsbns.Add(book.Isbn);
        var books = new List<Book> { book };

        // Act
        var response = await httpClient.GetAsync($"/books");
        var result = await response.Content.ReadFromJsonAsync<List<Book>>();

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        result.Should().BeEquivalentTo(books);
    }

    [Fact]
    public async Task GetBooks_ReturnsNoBooks_WhenNoBooksExists()
    {
        // Arrange
        var httpClient = _factory.CreateClient();

        // Act
        var response = await httpClient.GetAsync($"/books");
        var result = await response.Content.ReadFromJsonAsync<List<Book>>();

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetBooks_ReturnsBooks_WhenTitlesExist()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        var book = GenerateBook("Royal Assassin");
        await httpClient.PostAsJsonAsync("/books", book);
        _createdIsbns.Add(book.Isbn);
        var books = new List<Book> { book };

        // Act
        var response = await httpClient.GetAsync($"/books?searchTerm=oyal");
        var result = await response.Content.ReadFromJsonAsync<List<Book>>();

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        result.Should().BeEquivalentTo(books);
    }
    #endregion

    #region PUT
    [Fact]
    public async Task UpdateBook_UpdatesBook_WhenDataIsValid()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        var book = GenerateBook();
        await httpClient.PostAsJsonAsync("/books", book);
        _createdIsbns.Add(book.Isbn);

        // Act
        book.PageCount = 69;
        var response = await httpClient.PutAsJsonAsync($"/books/{book.Isbn}", book);
        var result = await response.Content.ReadFromJsonAsync<Book>();

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        result.Should().BeEquivalentTo(book);
    }

    [Fact]
    public async Task UpdateBook_DoesNotUpdateBook_WhenDataIsInvalid()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        var book = GenerateBook();
        await httpClient.PostAsJsonAsync("/books", book);
        _createdIsbns.Add(book.Isbn);

        // Act
        book.Title = string.Empty;
        var response = await httpClient.PutAsJsonAsync($"/books/{book.Isbn}", book);
        var errors = await response.Content.ReadFromJsonAsync<IEnumerable<ValidationError>>();
        var error = errors!.Single();

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        error.PropertyName.Should().Be("Title");
        error.ErrorMessage.Should().Be("'Title' must not be empty.");
    }

    [Fact]
    public async Task UpdateBook_ReturnsNotFound_WhenBookDoesNotExist()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        var book = GenerateBook();
       
        // Act
        var response = await httpClient.PutAsJsonAsync($"/books/{book.Isbn}", book);
        
        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }
    #endregion

    #region DELETE
    [Fact]
    public async Task DeleteBook_ReturnsNotFound_WhenBookDoesNotExist()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        var isbn = GenerateIsbn();

        // Act
        var response = await httpClient.DeleteAsync($"/books/{isbn}");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteBook_DeletesBook_WhenBookExists()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        var book = GenerateBook();
        await httpClient.PostAsJsonAsync("/books", book);

        // Act
        var response = await httpClient.DeleteAsync($"/books/{book.Isbn}");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
    }
    #endregion

    private Book GenerateBook(string title = "Assassin's Apprentice")
    {
        return new Book
        {
            Isbn = GenerateIsbn(),
            Title = title,
            Author = "Hobb, Robin",
            PageCount = 435,
            ShortDescription = "Young Fitz is the bastard son of the noble Prince Chivalry, raised in the shadow of the royal court by his fathers gruff stableman.",
            ReleaseDate = new DateTime(1996, 3, 1)
        };
    }

    private string GenerateIsbn()
    {
        return $"{Random.Shared.Next(100, 999)}-" +
            $"{Random.Shared.Next(1000000000, 2100999999)}";
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        var httpClient = _factory.CreateClient();
        foreach (var isbn in _createdIsbns)
        {
            await httpClient.DeleteAsync($"/books/{isbn}");
        }
    }
}
