using System.Net;
using ProductService.Core.Exceptions.Base;

namespace ProductService.Core.Exceptions.ServerErrors;

/// <summary>
///     Exception thrown when a dependent or external service is temporarily unavailable.
/// </summary>
public sealed class ServiceUnavailableException : ServerErrorException
{
    public override HttpStatusCode StatusCode => HttpStatusCode.ServiceUnavailable;

    public ServiceUnavailableException(string serviceName)
        : base($"Service '{serviceName}' is currently unavailable.", new { ServiceName = serviceName })
    {
    }

    public ServiceUnavailableException(string serviceName, Exception innerException)
        : base($"Service '{serviceName}' is currently unavailable.", innerException, new { ServiceName = serviceName })
    {
    }
}