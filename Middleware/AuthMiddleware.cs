namespace Middleware.TodoApi;

public class AuthMiddleware
{
    private readonly RequestDelegate _next;

    public AuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var auth = context.Request.Headers["Authorization"];
        if (string.IsNullOrWhiteSpace(auth))
        {
            var result = new {
                code = 403,
                message = "Not Authorized"
            };
            await context.Response.WriteAsJsonAsync(result);
        } else {    
            // Call the next delegate/middleware in the pipeline.
            await _next(context);
        }
    }
}

public static class AuthMiddlewareExtensions
{
    public static IApplicationBuilder UseAuth(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AuthMiddleware>();
    }
}