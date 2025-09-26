using System.Net;
using ProductService.Core.Exceptions.Base;

namespace ProductService.Core.Exceptions.ClientErrors;

/// <summary>
///     Exception thrown for bad or invalid client requests.
/// </summary>
public sealed class BadRequestException : ClientErrorException
{
    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;

    public BadRequestException(string message) : base(message)
    {
    }

    public BadRequestException(string message, object details) : base(message, details)
    {
    }
}