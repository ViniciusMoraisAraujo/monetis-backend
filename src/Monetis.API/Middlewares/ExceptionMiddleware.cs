using Monetis.Domain.Exceptions;

namespace Monetis.API.Middlewares;

public class ExceptionMiddleware(RequestDelegate next, IWebHostEnvironment env)
{

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (DomainException ex)
        {
            await HandleExceptionAsync(context, ex);
        }
        catch (Exception e)
        {
            await HandleExceptionAsync(context, e);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message, errorCode) = exception switch
        {
            DomainException => (StatusCodes.Status400BadRequest, exception.Message, "BUSINESS_ERROR"),
            ArgumentException => (StatusCodes.Status400BadRequest, "Bad Request", "04X0"),
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Not Found", "04X4"),
            _ => (StatusCodes.Status500InternalServerError, "Internal error", "07X0")
        };

        context.Response.StatusCode = statusCode;

        var problemDetails = new
        {
            StatusCode = statusCode,
            Message = message,
            ErrorCode = errorCode,
            Details = env.IsDevelopment() ? exception.ToString() : null,
            StackTrace = env.IsDevelopment() ? exception.StackTrace : null
        };

        await context.Response.WriteAsJsonAsync(problemDetails, context.RequestAborted);
    }
}
