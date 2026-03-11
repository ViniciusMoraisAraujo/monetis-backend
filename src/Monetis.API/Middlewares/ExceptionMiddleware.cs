namespace Monetis.API.Middlewares;

public class ExceptionMiddleware(RequestDelegate _next, ILogger<ExceptionMiddleware> _logger, IWebHostEnvironment _env)
{

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            await HandleExceptionAsync(context, e);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message, errorcode) = exception switch
        {
            ArgumentException => (StatusCodes.Status400BadRequest, "Bad Request", "04X0"),
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Not Found", "04X4"),
            _ => (StatusCodes.Status500InternalServerError, "Internal error", "07X0")
        };
        
        context.Response.StatusCode = statusCode;

        var problemDetails = new
        {
            StatusCode = statusCode,
            Message = message,
            ErrorCode = errorcode,
            Details = _env.IsDevelopment() ? exception.ToString() : null,
            StackTrace = _env.IsDevelopment() ? exception.StackTrace : null
        };

        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}