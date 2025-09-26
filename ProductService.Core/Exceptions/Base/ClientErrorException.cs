namespace ProductService.Core.Exceptions.Base;

/// <summary>
///     Base class for client error exceptions (4xx status codes)
/// </summary>
public abstract class ClientErrorException : BaseApplicationException
{
    protected ClientErrorException(string message) : base(message)
    {
    }

    protected ClientErrorException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected ClientErrorException(string message, object? details) : base(message, details)
    {
    }
}