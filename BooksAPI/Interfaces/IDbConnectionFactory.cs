using System.Data;

namespace BooksAPI.Interfaces;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync();
}
