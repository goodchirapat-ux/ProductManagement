namespace ProductManagement.Api.Middleware;

using ProductManagement.Core.Exceptions;
using System.Net;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            ProductNotFoundException =>
                new { error = exception.Message, statusCode = StatusCodes.Status404NotFound },
            InsufficientStockException =>
                new { error = exception.Message, statusCode = StatusCodes.Status400BadRequest },
            DuplicateSkuException =>
                new { error = exception.Message, statusCode = StatusCodes.Status409Conflict },
            InvalidPriceException =>
                new { error = exception.Message, statusCode = StatusCodes.Status400BadRequest },
            ArgumentException =>
                new { error = exception.Message, statusCode = StatusCodes.Status400BadRequest },
            _ => new { error = "An unexpected error occurred", statusCode = StatusCodes.Status500InternalServerError }
        };

        context.Response.StatusCode = response.statusCode;
        return context.Response.WriteAsJsonAsync(response);
    }
}
