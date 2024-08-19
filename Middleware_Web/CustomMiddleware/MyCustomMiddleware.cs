
namespace Middleware_Web.CustomMiddleware{
    public class MyCustomMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            await context.Response.WriteAsync("Custom middleware - start\n");
            await next(context);
            await context.Response.WriteAsync("Custom middleware - end\n");
        }
    }

    public static class MyCustomMiddlewareExtension{
        public static IApplicationBuilder UseMyCustomMiddleware(this IApplicationBuilder app){
            return app.UseMiddleware<MyCustomMiddleware>();
        }
    }
}

