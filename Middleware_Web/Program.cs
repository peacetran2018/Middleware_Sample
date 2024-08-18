using Middleware_Web.CustomMiddleware;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddTransient<MyCustomMiddleware>();

var app = builder.Build();
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

// //middleware 1
// app.Use(async (HttpContext context, RequestDelegate next) => {
//     await context.Response.WriteAsync("Hello\n");
//     await next(context);
// });
// //middleware 2
// // app.Use(async (HttpContext context1, RequestDelegate next) => {
// //     await context1.Response.WriteAsync("Hello again\n");
// //     await next(context1);
// // });
// //app.UseMiddleware<MyCustomMiddleware>();
// app.UseMyCustomMiddleware();
// app.UseHelloCustomMiddleware();

// //middleware 3
// app.Run(async (HttpContext context) => {
//     await context.Response.WriteAsync("Hello again again\n");
// });

app.Run();
