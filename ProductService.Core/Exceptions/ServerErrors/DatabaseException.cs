using System.Net;
using ProductService.Core.Exceptions.Base;

namespace ProductService.Core.Exceptions.ServerErrors;

/// <summary>
///     Exception thrown when a database operation fails unexpectedly.
/// </summary>
public sealed class DatabaseException : ServerErrorException
{
    public override HttpStatusCode StatusCode => HttpStatusCode.InternalServerError;

    public DatabaseException(string message) : base($"Database operation failed: {message}")
    {
    }

    public DatabaseException(string operation, Exception innerException)
        : base($"Database operation '{operation}' failed.", innerException, new { Operation = operation })
    {
    }
}