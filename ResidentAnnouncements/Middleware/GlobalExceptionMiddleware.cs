using RA_MuniAnnouncements.Api.Models;
using System.Diagnostics;
using System.Net;
using System.Text.Json;

namespace RA_MuniAnnouncements.Api.Middleware
{
    /// <summary>
    /// Global exception handling middleware that catches all unhandled exceptions,
    /// logs them appropriately, and returns standardized error responses.
    /// </summary>
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IHostEnvironment _environment;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var traceId = context.TraceIdentifier;
            var path = context.Request.Path;
            var method = context.Request.Method;

            // Determine status code and message based on exception type
            var (statusCode, message, details) = MapExceptionToResponse(exception);

            // Log the exception with appropriate level
            LogException(exception, statusCode, traceId, path, method);

            // Set response details
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            // Build error response
            var errorResponse = new ErrorResponse
            {
                TraceId = traceId,
                Message = message,
                Details = details,
                StatusCode = statusCode,
                Timestamp = DateTime.UtcNow,
                Exception = _environment.IsDevelopment() ? exception.GetType().Name : null
            };

            // Include inner exception details in development only
            if (_environment.IsDevelopment() && exception.InnerException != null)
            {
                errorResponse.Details = $"{exception.Message} | Inner: {exception.InnerException.Message}";
            }

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = _environment.IsDevelopment()
            };

            var json = JsonSerializer.Serialize(errorResponse, jsonOptions);
            return context.Response.WriteAsync(json);
        }

        private (int StatusCode, string Message, string? Details) MapExceptionToResponse(Exception exception)
        {
            return exception switch
            {
                // Client errors (4xx)
                ArgumentNullException => (
                    (int)HttpStatusCode.BadRequest,
                    "Required argument is missing.",
                    exception.Message
                ),
                ArgumentException => (
                    (int)HttpStatusCode.BadRequest,
                    "Invalid argument provided.",
                    exception.Message
                ),
                FormatException => (
                    (int)HttpStatusCode.BadRequest,
                    "Invalid data format.",
                    exception.Message
                ),
                OperationCanceledException => (
                    (int)HttpStatusCode.RequestTimeout,
                    "Request timeout occurred.",
                    "The operation took too long to complete."
                ),
                KeyNotFoundException => (
                    (int)HttpStatusCode.NotFound,
                    "Resource not found.",
                    exception.Message
                ),
                UnauthorizedAccessException => (
                    (int)HttpStatusCode.Unauthorized,
                    "Unauthorized access.",
                    "You do not have permission to access this resource."
                ),
                // Server errors (5xx) - unexpected/infrastructure
                _ => (
                    (int)HttpStatusCode.InternalServerError,
                    "An unexpected error occurred.",
                    exception.Message
                )
            };
        }

        private void LogException(Exception exception, int statusCode, string traceId, string path, string method)
        {
            var logLevel = GetLogLevel(statusCode);
            var eventId = new EventId(statusCode, GetEventName(statusCode));

            var message = $"[{method}] {path} | TraceId: {traceId} | StatusCode: {statusCode}";

            switch (logLevel)
            {
                case LogLevel.Error:
                    _logger.LogError(eventId, exception, message);
                    break;
                case LogLevel.Warning:
                    _logger.LogWarning(eventId, exception, message);
                    break;
                case LogLevel.Information:
                    _logger.LogInformation(eventId, message);
                    break;
                default:
                    _logger.LogDebug(eventId, exception, message);
                    break;
            }
        }

        private LogLevel GetLogLevel(int statusCode)
        {
            return statusCode switch
            {
                >= 500 => LogLevel.Error,
                >= 400 and < 500 => LogLevel.Warning,
                _ => LogLevel.Information
            };
        }

        private string GetEventName(int statusCode)
        {
            return statusCode switch
            {
                400 => "BadRequest",
                401 => "Unauthorized",
                403 => "Forbidden",
                404 => "NotFound",
                408 => "RequestTimeout",
                500 => "InternalServerError",
                503 => "ServiceUnavailable",
                _ => "UnexpectedException"
            };
        }
    }
}
