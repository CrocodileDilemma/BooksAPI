using BooksAPI.Extensions;
using Microsoft.AspNetCore.Cors;

namespace BooksAPI.Endpoints.Common;

internal partial class CommonEndpoints
{
    [EnableCors("AnyOrigin")]
    internal static async Task<IResult> GetStatus()
    {
        return Results.Extensions.Html(@"<!doctype html>
                <html>
                    <head><title>Status Page</title></head>
                    <body>
                        <h1>Status</h1>
                        <p>The server is working fine!</p>
                    </body>
                </html>");
    }
}
