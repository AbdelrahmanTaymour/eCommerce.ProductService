using System.Net;
using ProductService.Core.Exceptions.Base;

namespace ProductService.Core.Exceptions.ClientErrors;

/// <summary>
///     Exception thrown when trying to create a resource that already exists
/// </summary>
public sealed class ConflictException : ClientErrorException
{
    public override HttpStatusCode StatusCode => HttpStatusCode.Conflict;

    public ConflictException(string message) : base(message)
    {
    }

    public ConflictException(string entityName, object key)
        : base($"The {entityName} with the '{key}' already exists.",
            new { EntityName = entityName, Key = key })
    {
    }
}