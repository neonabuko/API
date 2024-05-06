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
                ? $"Parameter '{argNullEx.ParamName}' cannot be null."
                : "A required parameter was missing.";

            result = JsonSerializer.Serialize(new
            {
                title = errorMessage,
                status = (int)code,
                errors = new Dictionary<string, List<string>> { { argNullEx.ParamName, new List<string> { errorMessage } } }
            });
        }
        else if (ex is ConflictException e) // Custom exception for conflicts
        {
            code = HttpStatusCode.Conflict;
            var errorMessage = e.Message;

            result = JsonSerializer.Serialize(new
            {
                title = errorMessage,
                status = (int)code,
                errors = new Dictionary<string, List<string>> { { "Conflict", new List<string> { errorMessage } } }
            });
        }
        else if (ex is ValidationException validationEx)
        {
            code = HttpStatusCode.BadRequest;
            result = JsonSerializer.Serialize(new
            {
                title = validationEx.Message,
                status = (int)code,
                errors = GetValidationErrors(validationEx)
            });
        }

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)code;
        return context.Response.WriteAsync(result);
    }

    private static Dictionary<string, List<string>> GetValidationErrors(ValidationException validationEx)
    {
        var errors = new Dictionary<string, List<string>>();
        foreach (var error in validationEx.ValidationResult.MemberNames)
        {
            if (!errors.ContainsKey(error))
            {
                errors[error] = new List<string>();
            }
            errors[error].Add(validationEx.ValidationResult.ErrorMessage);
        }

        return errors;
    }
}

public class ConflictException(string message) : Exception(message)
{
}