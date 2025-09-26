using System.Net;
using ProductService.Core.Exceptions.Base;

namespace ProductService.Core.Exceptions.ClientErrors;

/// <summary>
///     Exception thrown when user is authenticated but lacks permission
/// </summary>
public sealed class ForbiddenException : ClientErrorException
{
    public override HttpStatusCode StatusCode => HttpStatusCode.Forbidden;

    public ForbiddenException() : base("You don't have permission to access this resource.")
    {
    }

    public ForbiddenException(string message) : base(message)
    {
    }

    public ForbiddenException(string resource, string action = "access")
        : base($"You do not have permission to {action} the {resource}.",
            new { Resource = resource, Action = action })
    {
    }
}