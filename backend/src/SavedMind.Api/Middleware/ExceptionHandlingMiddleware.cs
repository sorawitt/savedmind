using System.Net;
using System.Text.Json;
using FluentValidation;
using SavedMind.Application.Common.Exceptions;

namespace SavedMind.Api.Middleware;

public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message) = exception switch
        {
            ValidationException validationEx =>
                (HttpStatusCode.BadRequest, FormatValidationErrors(validationEx)),

            InvalidCredentialsException =>
                (HttpStatusCode.Unauthorized, "Invalid email or password."),

            EmailNotVerifiedException emailEx =>
                (HttpStatusCode.Forbidden, emailEx.Message),

            DuplicateEmailException dupEx =>
                (HttpStatusCode.BadRequest, dupEx.Message),

            AccountLockedException lockedEx =>
                (HttpStatusCode.Locked, lockedEx.Message),

            Application.Common.Exceptions.ApplicationException appEx =>
                (HttpStatusCode.BadRequest, appEx.Message),

            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
        };

        context.Response.StatusCode = (int)statusCode;

        var response = JsonSerializer.Serialize(new { error = message });
        await context.Response.WriteAsync(response);
    }

    private static string FormatValidationErrors(ValidationException ex)
    {
        var errors = ex.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray());

        return JsonSerializer.Serialize(new { errors });
    }
}