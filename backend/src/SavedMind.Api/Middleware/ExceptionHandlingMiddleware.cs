using System.Text.Json;
using FluentValidation;
using SavedMind.Application.Common.Exceptions;

namespace SavedMind.Api.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        logger.LogError(exception, "An error occurred: {Message}", exception.Message);

        var (statusCode, errors) = exception switch
        {
            // Application exceptions (e.g., duplicate email)
            DuplicateEmailException e => (StatusCodes.Status400BadRequest, new[] { e.Message }),

            // FluentValidation errors
            ValidationException e => (StatusCodes.Status400BadRequest,
                e.Errors.Select(err => err.ErrorMessage).Distinct().ToArray()),

            // Everything else = 500
            _ => (StatusCodes.Status500InternalServerError, new[] { "An unexpected error occurred." })
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = JsonSerializer.Serialize(new { errors });
        await context.Response.WriteAsync(response);
    }
}