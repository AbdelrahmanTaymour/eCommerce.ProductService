using System.Text.Json.Serialization;

namespace ProductService.Core.DTOs.ExceptionsDTOs;


/// <summary>
///     Represents a structured error response for client-side consumption
/// </summary>
public record ExceptionResponseDto
{
    public string Message { get; init; }
    public string ErrorCode { get; init; }
    public string ExceptionType { get; init; }
    public int StatusCode { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? Details { get; init; }

    public string TraceId { get; init; }
    public DateTime Timestamp { get; init; }

    public ExceptionResponseDto(
        string message,
        string errorCode,
        string exceptionType,
        int statusCode,
        object? details = null,
        string? traceId = null
    )
    {
        Message = message;
        ErrorCode = errorCode;
        ExceptionType = exceptionType;
        StatusCode = statusCode;
        Details = details;
        TraceId = traceId ?? Guid.NewGuid().ToString();
        Timestamp = DateTime.UtcNow;
    }
}