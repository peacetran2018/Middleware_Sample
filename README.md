## 1. What is Middleware?
  - Middleware is a component that is assembled into the application pipeline to handle requests and response.
## 2. Middleware - Run
  - The extension method called "Run" is used to execute a terminating / short-circuiting middleware that doesn't forward the request to the next middleware.
  - There is 1 parameter: HttpContext
### Syntax
```C#
app.Run(async (HttpContext context) =>{
  //Logic
});
```
### Usage
```C#
//Middleware 1
app.Run(async (HttpContext context) =>{
  context.Response.WriteAsync("Hello World");
});
```
## 3. How to run multiple middleware?
  - To middleware can run subsequent we can use app.Use instead of app.Run
  - There are 2 parameters: HttpContext and RequestDelegate
### Syntax
```C#
app.Use(async (HttpContext context, RequestDelate next) => {
  //Logic
  await next(context);//this method will be executed next middleware
  //Another logic
});
```
### Usage
```C#
//Middleware 1
app.Use(async (HttpContext context, RequestDelate next) => {
  await context.Response.WriteAsync("From middleware 1\n");
  await next(context);//this method will be executed next middleware
});

//Middleware 2
app.Run(async (HttpContext context) => {
  await context.Response.WriteAsync("From middleware 2");
});
```
### Output
```Text
From middleware 1
From middleware 2
```
## 4. How to custom middleware?
  - Create a C# class and inheritance from IMiddleware
  - Implement InvokeAsync method with 2 parameters HttpContext, RequestDelegate
### Syntax
```C#
public class MyCustomMiddleware : IMiddleware{
      public async Task InvokeAsync(HttpContext context, RequestDelegate next){
          //Logic
      }
  }
```
### Ussage
```C#
//MyCustomMiddleware.cs
namespace CustomMiddleware{
  public class MyCustomMiddleware : IMiddleware{
      public async Task InvokeAsync(HttpContext context, RequestDelegate next){
          await context.Response.WriteAsync("From middleware 2 - start\n");
          await next(context);
          await context.Response.WriteAsync("From middleware 2 - end\n");
      }
  }
}
//Program.cs
var builder = WebApplication.CreateBuilder(args);

//Add Service for Custom middleware class
builder.Services.AddTransient<MyCustomMiddleware>();

var app = builder.Build();
//middleware 1
app.Use(async (HttpContext context, RequestDelegate next) => {
    await context.Response.WriteAsync("From Middleware 1\n");
    await next(context);
});
//middleware 2
app.UseMiddleware<MyCustomMiddleware>();

//middleware 3
app.Run(async (HttpContext context) => {
    await context.Response.WriteAsync("From Middleware 3\n");
});
```
### Output
```Text
From middleware 1
From middleware 2 - start
From middleware 3
From middleware 2 - end
```
## 5. How to custom middleware with extensions?
  - Extesion method is a method that inject into an object dynamic.
### Usage
```C#
//MyCustomMiddleware.cs
namespace CustomMiddleware{
  ...
  public static class MyCustomMiddlewareExtension{
    public static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder app){
      return app.UseMiddleware<MyCustomMiddleware>();
    }
  }
}

//Program.cs
var builder = WebApplication.CreateBuilder(args);

//Add Service for Custom middleware class
builder.Services.AddTransient<MyCustomMiddleware>();

var app = builder.Build();
//middleware 1
app.Use(async (HttpContext context, RequestDelegate next) => {
    await context.Response.WriteAsync("From Middleware 1\n");
    await next(context);
});
//middleware 2
//app.UseMiddleware<MyCustomMiddleware>();
//Replace by extension
app.UseCustomMiddleware();

//middleware 3
app.Run(async (HttpContext context) => {
    await context.Response.WriteAsync("From Middleware 3\n");
});
```
### Output
```Text
From Middleware 1
From middleware 2 - start
From middleware 3
From middleware 2 - end
```
## 6. How to custom conventional middleware?
  - Instead we inherited from IMiddleware interface. We can create Middleware class from visual studio
  - This one NO NEED to add to service in program.cs
### Syntax
```C#
namespace NameSpaceName {
  public class MiddlewareClassName{
    private readonly RequestDelegate _next;
    public MiddlewareClassName(RequestDelegate next){
      _next = next;
    }

    public async Task InvokeAsync(HttpContext context){
      //before logic
      await _next(context);
      //after logic
    }
  }
}
```

### Usage
```C#
//HelloCustomMiddleware.cs
namespace NameSpaceName {
  public class HelloCustomMiddleware{
    private readonly RequestDelegate _next;
    public MiddlewareClassName(RequestDelegate next){
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

//Program.cs
var builder = WebApplication.CreateBuilder(args);

//Add Service for Custom middleware class
builder.Services.AddTransient<MyCustomMiddleware>();

var app = builder.Build();
//middleware 1
app.Use(async (HttpContext context, RequestDelegate next) => {
    await context.Response.WriteAsync("From Middleware 1\n");
    await next(context);
});
//middleware 2
//app.UseMiddleware<MyCustomMiddleware>();
//Replace by extension
app.UseCustomMiddleware();
//Add new extension
app.UseHelloCustomMiddleware();

//middleware 3
app.Run(async (HttpContext context) => {
    await context.Response.WriteAsync("From Middleware 3\n");
});
```
### Output
```Text
URL: localhost/?FirstName=An&LastName=Tran

From Middleware 1
From middleware 2 - start
An Tran Start
From Middleware 3
An Tran End
From middleware 2 - end
```
## 7. Middlware - UseWhen
### Usage
```C#
//Program.cs
...
var app = buidler.Build();
app.UseWhen((context) => 
    context.Request.Query.ContainsKey("username"),
    app => {
        app.Use(async (HttpContext context, RequestDelegate next) => {
            await context.Response.WriteAsync("Hello from middleware branch\n");
            await next(context);
        });
    }
);

app.Run(async (HttpContext context) =>{
    await context.Response.WriteAsync("Hello from middleware at main chain");
});
```
### Output
```Text
URL: localhost/?username=abc

Hello from middleware branch
Hello from middleware at main chain
```
## 8. Right Order of Middleware
```C#
app.UseExceptionHandler("/Error");
app.UseHsts();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapControllers();
//Add your custom middlwares
app.Run();
```
