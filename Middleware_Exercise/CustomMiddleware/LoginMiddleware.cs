using Microsoft.Extensions.Primitives;

namespace Middleware_Exercise.CustomMiddleware;

public class LoginMiddleware
{
    private readonly RequestDelegate _next;
    public LoginMiddleware(RequestDelegate next){
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context){
        if(context.Request.Path == "/" && context.Request.Method == "POST"){
            StreamReader streamReader = new StreamReader(context.Request.Body);
            string body = await streamReader.ReadToEndAsync();

            Dictionary<string, StringValues> queryDict = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(body);
            string? email = null;
            string? password = null;

            if(queryDict.ContainsKey("email")){
                email = queryDict["email"];
            }
            else{
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Invalid input for 'email'\n");
            }

            if(queryDict.ContainsKey("password")){
                password = queryDict["password"];
            }
            else{
                if(context.Response.StatusCode == 200){
                    context.Response.StatusCode = 400;
                }
                await context.Response.WriteAsync("Invalid input for 'password'\n");
            }


            if(!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password)){
                const string validEmail = "admin@example.com";
                const string validPassword = "admin1234";
                if(email == validEmail && password == validPassword){
                    context.Response.StatusCode = 200;
                    await context.Response.WriteAsync("Sucessful login");
                }
                else{
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("Invalid login");
                }
            }
        }
        else{
            await _next(context);
        }
    }
}

public static class LoginMiddlewareExtension{
    public static IApplicationBuilder UseLoginMiddleware(this IApplicationBuilder builder){
        return builder.UseMiddleware<LoginMiddleware>();
    }
}