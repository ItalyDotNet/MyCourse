namespace MyCourse.Customizations.Middlewares;

public class TrackingMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<TrackingMiddleware> logger;

    public TrackingMiddleware(RequestDelegate next, ILogger<TrackingMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string trackingId = context.Request.Cookies["TrackingId"];
        if (trackingId is null or "")
        {
            trackingId = Guid.NewGuid().ToString();
            context.Response.Cookies.Append("TrackingId", trackingId, new CookieOptions { IsEssential = false, Expires = DateTimeOffset.Now.AddDays(30) });
        }

        logger.LogInformation($@"
        At {DateTimeOffset.Now}
        user {context.User.Identity.Name ?? "Anonimo"}
        with tracking ID {trackingId}
        from IP {context.Connection.RemoteIpAddress}
        visited {context.Request.Method} {context.Request.Path}
        with preferred language {context.Request.Headers.AcceptLanguage}
        coming from page {context.Request.Headers.Referer}");

        await next(context);
    }
}
