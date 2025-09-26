using System.Net;
using ProductService.Core.Exceptions.Base;

namespace ProductService.Core.Exceptions.ServerErrors;

/// <summary>
///     Exception thrown when a downstream external dependency fails.
/// </summary>
public sealed class ExternalDependencyException : ServerErrorException
{
    public override HttpStatusCode StatusCode => HttpStatusCode.BadGateway;

    public ExternalDependencyException(string dependencyName, string message)
        : base($"External dependency '{dependencyName}' failed: {message}", new { DependencyName = dependencyName })
    {
    }

    public ExternalDependencyException(string dependencyName, Exception innerException)
        : base($"External dependency '{dependencyName}' failed.", innerException,
            new { DependencyName = dependencyName })
    {
    }
}