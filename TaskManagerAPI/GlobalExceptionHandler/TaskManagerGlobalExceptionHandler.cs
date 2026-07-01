using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TaskManagerAPI.GlobalExceptionHandler
{
    public class TaskManagerGlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<TaskManagerGlobalExceptionHandler> _logger;
        private readonly IHostEnvironment _environment;

        public TaskManagerGlobalExceptionHandler(ILogger<TaskManagerGlobalExceptionHandler> logger, IHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            LogException(httpContext, exception);

            var problemDetails = GetProblemDetails(httpContext, exception);

            httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }

        private ProblemDetails GetProblemDetails(HttpContext httpContext, Exception exception) 
        {
            var problemDetails = exception switch
            {
                UnauthorizedAccessException => new ProblemDetails
                {
                    Title = "Unauthorized access",
                    Detail = exception.Message,
                    Status = StatusCodes.Status401Unauthorized,
                    Type = "https://httpstatuses.com/401"
                },

                ArgumentException => new ProblemDetails
                {
                    Title = "Invalid argument",
                    Detail = exception.Message,
                    Status = StatusCodes.Status400BadRequest,
                    Type = "https://httpstatuses.com/400"

                },

                /* 
                   KeyNotFoundException is currently used both for invalid application configuration (HTTP 500) and for resource not found scenarios (HTTP 404).
                   
                   In the next commit, it will be replaced with custom exceptions to allow different HTTP status codes.
                */
                KeyNotFoundException => new ProblemDetails
                {
                    Title = "Resource not found",
                    Detail = exception.Message,
                    Status = StatusCodes.Status404NotFound,
                    Type = "https://httpstatuses.com/404"
                },

                /* 
                    InvalidOperationException is currently used for invalid TokenExpirationInHours configuration (HTTP 500), failed user or role creation (HTTP 500), and invalid operations during request processing (HTTP 400).

                    In the next commit, it will be replaced with custom exceptions to allow different HTTP status codes.
                */
                InvalidOperationException => new ProblemDetails
                {
                    Title = "Invalid operation",
                    Detail = exception.Message,
                    Status = StatusCodes.Status500InternalServerError,
                    Type = "https://httpstatuses.com/500"
                },

                DbUpdateConcurrencyException => new ProblemDetails
                {
                    Title = "Database concurrency conflict",
                    Detail = exception.Message,
                    Status = StatusCodes.Status500InternalServerError,
                    Type = "https://httpstatuses.com/500"
                },

                _ => new ProblemDetails
                {
                    Title = "Internal server error",
                    Detail = _environment.IsDevelopment() ? exception.ToString() : "An unexpected error occurred.",
                    Status = StatusCodes.Status500InternalServerError,
                    Type = "https://httpstatuses.com/500"

                }
            };

            problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

            return problemDetails;
        }


        private void LogException(HttpContext httpContext, Exception exception)
        {
            switch (exception) 
            {
                case UnauthorizedAccessException ex:
                    _logger.LogWarning(ex, "{Message}. TraceId: {TraceId}", ex.Message, httpContext.TraceIdentifier);
                    break;
                case ArgumentException ex:
                    _logger.LogWarning(ex, "{Message}. TraceId: {TraceId}", ex.Message, httpContext.TraceIdentifier);
                    break;
                case KeyNotFoundException ex:
                    _logger.LogWarning(ex, "{Message}. TraceId: {TraceId}", ex.Message, httpContext.TraceIdentifier);
                    break;
                case InvalidOperationException ex:
                    _logger.LogError(ex, "{Message}. TraceId: {TraceId}", ex.Message, httpContext.TraceIdentifier);
                    break;
                case DbUpdateConcurrencyException ex:
                    _logger.LogError(ex, "Database concurrency conflict. TraceId: {TraceId}", httpContext.TraceIdentifier);
                    break;
                default:
                    _logger.LogError(exception, "Unhandled exception occurred. Message: {Message}. TraceId: {TraceId}", exception.Message, httpContext.TraceIdentifier);
                    break;
            }
        }
    }
}
