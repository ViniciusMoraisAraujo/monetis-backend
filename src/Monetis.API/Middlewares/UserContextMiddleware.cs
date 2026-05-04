using System.Security.Claims;
using Monetis.Infrastructure.Contexts;

namespace Monetis.API.Middlewares;

public class UserContextMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext httpContext, UserContext userContext)
    {
        var claim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)
                    ?? httpContext.User.FindFirst("sub");
        if (claim != null && Guid.TryParse(claim.Value, out var userId))
        {
            userContext.SetUser(userId);
        }

        await next(httpContext);
    }
}