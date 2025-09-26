using System.Net;
using System.Text.Json;
using FluentValidation;
using ProductService.Core.DTOs.ExceptionsDTOs;
using ProductService.Core.Exceptions.Base;
using ClientErrors = ProductService.Core.Exceptions.ClientErrors;


namespace eCommerce.ProductService.Middleware;

/// <summary>
///     Middleware for centralized exception handling in the application.
///     Catches and processes all unhandled exceptions, converting them into standardized API responses.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ExceptionHandlingMiddleware" /> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="logger">The logger instance for recording exception details.</param>
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    /// <summary>
    ///     Processes an HTTP request and handles any exceptions that occur.
    /// </summary>
    /// <param name="httpContext">The context for the current HTTP request.</param>
    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    /// <summary>
    ///     Handles exceptions by converting them into appropriate HTTP responses.
    /// </summary>
    /// <param name="context">The context for the current HTTP request.</param>
    /// <param name="exception">The exception to handle.</param>
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = context.TraceIdentifier;

        // Handle custom application exceptions
        if (exception is BaseApplicationException appException)
        {
            await HandleApplicationExceptionAsync(context, appException, traceId);
            return;
        }

        // Handle FluentValidation exceptions
        if (exception is ValidationException fluentValidationEx)
        {
            var customValidationEx = new ClientErrors.ValidationException(fluentValidationEx);
            await HandleApplicationExceptionAsync(context, customValidationEx, traceId);
            return;
        }

        // Handle specific system exceptions
        var response = exception switch
        {
            ArgumentNullException =>
                CreateErrorResponse("Required parameter is missing.", "MissingParameter", HttpStatusCode.BadRequest,
                    traceId),

            ArgumentException argEx =>
                CreateErrorResponse(argEx.Message, "ArgumentError", HttpStatusCode.BadRequest, traceId),

            InvalidOperationException invalidOpEx =>
                CreateErrorResponse(invalidOpEx.Message, "InvalidOperation", HttpStatusCode.BadRequest, traceId),

            UnauthorizedAccessException unauthenticEx => CreateErrorResponse(
                unauthenticEx.Message, "AccessDenied", HttpStatusCode.Forbidden, traceId),

            TimeoutException => CreateErrorResponse(
                "The operation timed out.", "Timeout", HttpStatusCode.RequestTimeout, traceId),

            TaskCanceledException => CreateErrorResponse(
                "The operation was cancelled.", "OperationCancelled", HttpStatusCode.RequestTimeout, traceId),

            _ => CreateErrorResponse(
                "An unexpected error occurred.", "InternalError", HttpStatusCode.InternalServerError, traceId)
        };


        // Log the exception
        LogException(exception, response.StatusCode, traceId);

        // Send response
        await SendErrorResponseAsync(context, response);
    }

    /// <summary>
    ///     Handles application-specific exceptions by converting them into appropriate HTTP responses.
    /// </summary>
    /// <param name="context">The context for the current HTTP request.</param>
    /// <param name="exception">The application-specific exception to handle.</param>
    /// <param name="traceId">The unique identifier for tracing the request.</param>
    private async Task HandleApplicationExceptionAsync(HttpContext context, BaseApplicationException exception,
        string traceId)
    {
        ExceptionResponseDto response;

        // Special handling for validation exceptions
        if (exception is ClientErrors.ValidationException validationEx)
        {
            response = new ValidationErrorResponseDto(
                validationEx.Message,
                validationEx.ValidationErrors,
                traceId);

            _logger.LogWarning("Validation failed: {Message} | TraceId: {TraceId} | Details: {@Details}",
                validationEx.Message, traceId, validationEx.ValidationErrors);
        }
        else
        {
            response = new ExceptionResponseDto(
                exception.Message,
                exception.ErrorCode,
                exception.GetType().Name,
                (int)exception.StatusCode,
                exception.Details,
                traceId);

            // Log based on severity
            if ((int)exception.StatusCode >= 500)
                _logger.LogError(exception, "Server error occurred: {Message} | TraceId: {TraceId}",
                    exception.Message, traceId);
            else
                _logger.LogWarning("Client error occurred: {Message} | TraceId: {TraceId} | StatusCode: {StatusCode}",
                    exception.Message, traceId, (int)exception.StatusCode);
        }

        await SendErrorResponseAsync(context, response);
    }

    /// <summary>
    ///     Creates a standardized error response.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="errorCode">The error code identifier.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="traceId">The unique identifier for tracing the request.</param>
    /// <returns>A standardized exception response DTO.</returns>
    private static ExceptionResponseDto CreateErrorResponse(string message, string errorCode, HttpStatusCode statusCode,
        string traceId)
    {
        return new ExceptionResponseDto(message, errorCode, "SystemException", (int)statusCode, null, traceId);
    }

    /// <summary>
    ///     Logs exception details with the appropriate severity level.
    /// </summary>
    /// <param name="exception">The exception to log.</param>
    /// <param name="statusCode">The HTTP status code associated with the error.</param>
    /// <param name="traceId">The unique identifier for tracing the request.</param>
    private void LogException(Exception exception, int statusCode, string traceId)
    {
        if (statusCode >= 500)
            _logger.LogError(exception, "Unhandled exception occurred: {Message} | TraceId: {TraceId}",
                exception.Message, traceId);
        else
            _logger.LogWarning("Exception occurred: {Message} | TraceId: {TraceId} | StatusCode: {StatusCode}",
                exception.Message, traceId, statusCode);
    }

    /// <summary>
    ///     Sends the error response to the client.
    /// </summary>
    /// <param name="context">The context for the current HTTP request.</param>
    /// <param name="response">The error response to send.</param>
    private async Task SendErrorResponseAsync(HttpContext context, ExceptionResponseDto response)
    {
        context.Response.StatusCode = response.StatusCode;
        context.Response.ContentType = "application/json";

        var jsonResponse = JsonSerializer.Serialize(response, _jsonOptions);
        await context.Response.WriteAsync(jsonResponse);
    }
}