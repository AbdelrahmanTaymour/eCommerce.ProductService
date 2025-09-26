using System.Net;

namespace ProductService.Core.Exceptions.Base;

/// <summary>
/// Base class for all custom application exceptions
/// </summary>
public abstract class BaseApplicationException : Exception
{
    public abstract HttpStatusCode StatusCode { get; }
    public virtual string ErrorCode => GetType().Name.Replace("Exception", "");
    public virtual object? Details { get; }

    protected BaseApplicationException(string message) : base(message)
    {
    }

    protected BaseApplicationException(string message, Exception innerException, object? details = null)
        : base(message, innerException)
    {
        Details = details;
    }

    protected BaseApplicationException(string message, object? details) : base(message)
    {
        Details = details;
    }
}