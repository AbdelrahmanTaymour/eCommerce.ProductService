namespace ProductService.Core.Exceptions.Base;

/// <summary>
///     Base class for server error exceptions (5xx status codes)
/// </summary>
public abstract class ServerErrorException : BaseApplicationException
{
    protected ServerErrorException(string message) : base(message)
    {
    }

    protected ServerErrorException(string message, Exception innerException, object? details = null)
        : base(message, innerException, details)
    {
    }

    protected ServerErrorException(string message, object? details) : base(message, details)
    {
    }
}