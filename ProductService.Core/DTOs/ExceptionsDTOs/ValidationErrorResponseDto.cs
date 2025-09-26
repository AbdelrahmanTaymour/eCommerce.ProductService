using ProductService.Core.Exceptions.ClientErrors;

namespace ProductService.Core.DTOs.ExceptionsDTOs;

/// <summary>
///     Represents a structured error response for validation failures,
///     including field-specific error messages
/// </summary>
public record ValidationErrorResponseDto : ExceptionResponseDto
{
    public Dictionary<string, string[]> ValidationErrors { get; init; }

    public ValidationErrorResponseDto(
        string message,
        Dictionary<string, string[]> validationErrors,
        string? traceId = null)
        : base(message, "ValidationError", nameof(ValidationException), 400, validationErrors, traceId)
    {
        ValidationErrors = validationErrors;
    }
}