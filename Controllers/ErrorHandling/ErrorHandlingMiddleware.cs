using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Web;

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
        HttpStatusCode code = HttpStatusCode.InternalServerError;
        string result = JsonSerializer.Serialize(new
        {
            title = "An error occurred. Please try again later.",
            status = (int)code,
            errors = new Dictionary<string, List<string>>()
        });

        if (ex is ArgumentNullException argNullEx)
        {
            code = HttpStatusCode.BadRequest;
            var errorMessage = argNullEx.ParamName is not null
                ? $"Parameter '{argNullEx.ParamName}' must be provided."
                : "A required parameter was missing.";

            result = JsonSerializer.Serialize(new
            {
                title = errorMessage,
                status = (int)code,
                errors = new Dictionary<string, string> {{ "Title", errorMessage }}
            });
        }
        else if (ex is ConflictException e)
        {
            code = HttpStatusCode.Conflict;
            var errorMessage = e.Message;

            result = JsonSerializer.Serialize(new
            {
                title = errorMessage,
                status = (int)code,
                errors = new Dictionary<string, string> {{ "Title", errorMessage }}
            });
        }
        else if (ex is ValidationException validationEx)
        {
            code = HttpStatusCode.BadRequest;
            result = JsonSerializer.Serialize(new
            {
                title = validationEx.Message,
                status = (int)code,
                errors = new Dictionary<string, string> {{ "Title", validationEx.Message }}
            });
        }

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)code;
        return context.Response.WriteAsync(result);
    }
}

public class ConflictException(string message) : Exception(message)
{
}