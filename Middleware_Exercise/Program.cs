using Middleware_Exercise.CustomMiddleware;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseLoginMiddleware();
app.MapGet("/", () => "Hello World!");
app.Run();
