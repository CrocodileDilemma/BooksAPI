namespace BooksAPI.Entities;

public class Book
{
    public string Isbn { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string ShortDescription { get; set; } = default!;
    public string Author { get; set; } = default!;
    public int PageCount { get; set; } = default!;
    public DateTime ReleaseDate { get; set; } = default!;
}
