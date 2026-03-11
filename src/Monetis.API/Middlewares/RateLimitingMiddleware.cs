using Microsoft.Extensions.Caching.Memory;

namespace Monetis.API.Middlewares;

public class RateLimitingMiddleware(RequestDelegate next, IMemoryCache cache, ILogger<RateLimitingMiddleware> logger)
{
    private const int RequestLimit = 5;
    private static readonly TimeSpan TimeInterval = TimeSpan.FromSeconds(10);

    public async Task InvokeAsync(HttpContext context)
    {
        var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var keyCache = $"RateLimit_{clientIp}";

        if (cache.TryGetValue(keyCache, out int rateLimit))
        {
            if (rateLimit >= RequestLimit)
            {
                logger.LogWarning("Limit exceeded for IP address {IP}", clientIp);
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync("You have exceeded the limit of attempts. Please try again later");
                return;
            }
            
            cache.Set(keyCache, rateLimit + 1);
        }
        else
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeInterval
            };
            
            cache.Set(keyCache, 1, TimeInterval);
        }
        await next(context);
    }
}