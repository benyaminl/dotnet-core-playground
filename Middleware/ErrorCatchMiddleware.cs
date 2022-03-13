using System.Net;
using System.Text.Json;

namespace TodoApi.Middleware;
/**
    @see https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/write?view=aspnetcore-6.0#middleware-dependencies
*/
public class ErrorCatchMiddleware {
    private readonly RequestDelegate _next;
    // @see https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.hosting.ihostingenvironment?view=aspnetcore-6.0
    // @see https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.hosting.iwebhostenvironment?view=aspnetcore-6.0
    private readonly IWebHostEnvironment env;

    public ErrorCatchMiddleware(RequestDelegate next, IWebHostEnvironment env)
    {
        this.env = env;
        this._next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception error)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            switch(error)
            {
                case KeyNotFoundException e:
                    // not found error
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;
                default:
                    // unhandled error
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }
            
            string result;
            if (env.IsDevelopment()) {
                result = JsonSerializer.Serialize(new { 
                    code = response.StatusCode, 
                    message = error?.Message, 
                    // To String because can't be encoded
                    exception = error?.GetType().ToString(),
                    // to multiple array, so still human readable per row
                    errorTrace = error?.StackTrace?.Split("\n"),
                    // Only used when, we use double throw
                    sourceError = new {
                        message = error?.InnerException?.Message,
                        sourceException = error?.InnerException?.GetType().ToString(),
                        sourceTrace =  error?.InnerException?.StackTrace?.Split("\n").Reverse()
                    }
                });
            } else {
                result = JsonSerializer.Serialize(new { 
                    code = response.StatusCode, 
                    message = error?.Message, 
                    exception = error?.GetType().ToString()
                });
            }

            await response.WriteAsync(result);
        }
    }
}