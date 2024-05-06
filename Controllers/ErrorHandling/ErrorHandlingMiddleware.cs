using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace ScoreHubAPI.Controllers.ErrorHandling;

public class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger = logger;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var (statusCode, errorMessage) = ex switch
        {
            ArgumentNullException argNullEx => (HttpStatusCode.BadRequest, argNullEx.Message),
            ConflictException conflictEx => (HttpStatusCode.Conflict, conflictEx.Message),
            ValidationException validationEx => (HttpStatusCode.BadRequest, validationEx.Message),
            _ => (HttpStatusCode.InternalServerError, "An error occurred. Please try again later.")
        };

        var responseContent = CreateErrorResponse(statusCode, errorMessage);

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;

        return context.Response.WriteAsync(JsonSerializer.Serialize(responseContent));
    }

    private static object CreateErrorResponse(HttpStatusCode statusCode, string errorMessage)
    {
        return new
        {
            title = errorMessage,
            Status = (int)statusCode,
            errors = new Dictionary<string, string> { { "Title", errorMessage } }
        };
    }
}

public class ConflictException(string message) : Exception(message)
{
}