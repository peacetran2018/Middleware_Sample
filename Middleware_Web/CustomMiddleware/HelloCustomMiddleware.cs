namespace Middleware_Web.CustomMiddleware{

public class HelloCustomMiddleware
{
    private readonly RequestDelegate _next;
    public HelloCustomMiddleware(RequestDelegate next){
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context){
        //before logic
        if(context.Request.Query.ContainsKey("FirstName") && context.Request.Query.ContainsKey("LastName")){
            string fullName = context.Request.Query["FirstName"] + " " + context.Request.Query["LastName"] + " Start\n";
            await context.Response.WriteAsync(fullName);
        }
        await _next(context);
        //after logic
        if(context.Request.Query.ContainsKey("FirstName") && context.Request.Query.ContainsKey("LastName")){
            string fullName = context.Request.Query["FirstName"] + " " + context.Request.Query["LastName"] + " End\n";
            await context.Response.WriteAsync(fullName);
        }
    }
}

public static class HelloCustomMiddlewareExtension{
    public static IApplicationBuilder UseHelloCustomMiddleware(this IApplicationBuilder app){
        return app.UseMiddleware<HelloCustomMiddleware>();
    }
}
}