using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.GlobalExceptionHandler.Exceptions;
using TaskManagerAPI.GlobalExceptionHandler.Exceptions.Business;
using TaskManagerAPI.GlobalExceptionHandler.Exceptions.Identity;

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
                TaskManagerException ex => new ProblemDetails
                {
                    Title = ex.Title,
                    Detail = ex.Message,
                    Status = ex.StatusCode,
                    Type = ex.Type
                },

                DbUpdateConcurrencyException ex => new ProblemDetails
                {
                    Title = "Database concurrency conflict",
                    Detail = ex.Message,
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

            if (exception is IdentityOperationFailedException exc)
                problemDetails.Extensions["errors"] = exc.Errors;

            return problemDetails;
        }

        private void LogException(HttpContext httpContext, Exception exception)
        {
            switch (exception)
            {
                //Warnings
                case UserAlreadyExistsException ex:
                    _logger.LogWarning(ex, "User with email {Email} already exists. TraceId: {TraceId}", ex.Email, httpContext.TraceIdentifier);
                    break;

                case InvalidCredentialsException ex:
                    _logger.LogWarning(ex, "Authentication Failed. Invalid email or password. TraceId: {TraceId}", httpContext.TraceIdentifier);
                    break;

                case NotFoundException ex:
                    _logger.LogWarning(ex, "{ResourceName} with value {ResourceValue} not found. TraceId: {TraceId}", ex.ResourceName, ex.ResourceValue, httpContext.TraceIdentifier);
                    break;

                case ParameterValidationException ex:
                    _logger.LogWarning(ex, "{Message}. ParameterName: {ParameterName}. ParameterValue: {ParameterValue}. TraceId: {TraceId}", ex.Message, ex.ParameterName, ex.ParameterValue, httpContext.TraceIdentifier);
                    break;

                case ForbiddenOperationException ex:
                    _logger.LogWarning(ex, "{Message}. ResourceName: {ResourceName}. ResourceValue: {ResourceValue}. TraceId: {TraceId}",
                        ex.Message, ex.ResourceName, ex.ResourceValue, httpContext.TraceIdentifier);
                    break;

                case UnauthorizedException ex:
                    _logger.LogWarning(ex, "{Message}. TraceId: {TraceId}", ex.Message, httpContext.TraceIdentifier);
                    break;

                case ArgumentMismatchException ex:
                    _logger.LogWarning(ex, "{ExpectedName} value '{ExpectedValue}' does not match {ActualName} value '{ActualValue}'. TraceId: {TraceId}", ex.ExpectedName, ex.ExpectedValue, ex.ActualName, ex.ActualValue, httpContext.TraceIdentifier);
                    break;

                //Errors
                case UserCreationFailedException ex:
                    _logger.LogError(ex, "Failed to create user with email {Email}. Errors: {Errors}. TraceId: {TraceId}", ex.Email, string.Join(", ", ex.Errors), httpContext.TraceIdentifier);
                    break;

                case RoleCreationFailedException ex:
                    _logger.LogError(ex, "Failed to create role {Role}. Errors: {Errors}. TraceId: {TraceId}", ex.Role, string.Join(", ", ex.Errors), httpContext.TraceIdentifier);
                    break;

                case RoleAssignmentFailedException ex:
                    _logger.LogError(ex, "Failed to assign role {Role} to user with email {Email}. Errors: {Errors}. TraceId: {TraceId}", ex.Role, ex.Email, string.Join(", ", ex.Errors), httpContext.TraceIdentifier);
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