using CompanyEmployees.Extensions;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using NLog;
using Repository;
using System.ComponentModel;

var builder = WebApplication.CreateBuilder(args);

// configuration for a logger service
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var configFileName = $"nlog.{environment}.config";
var configFilePath = Path.Combine(Directory.GetCurrentDirectory(), configFileName);

LogManager.Setup().LoadConfigurationFromFile(configFilePath);

// Use the ConfigureCors extension method
builder.Services.ConfigureCors();

// IIS integration
builder.Services.ConfigureIISIntegration();

// Logger service
builder.Services.ConfigureLoggerService();

// Add services to the container.
builder.Services.AddControllers();

//Repository pattern separating the logic from model
builder.Services.ConfigureRepositoryManager();
//encapsulates the registration of all service classes togehter in a service manager
builder.Services.ConfigureServiceManager();

// SQL server connection
builder.Services.ConfigureSqlContext(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.

//handling exceptions and HTTP Strict Transport Security (HSTS)
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
}
//automatically redirects HTTP requests to HTTPS
app.UseHttpsRedirection();

// enables using static files for the request.
app.UseStaticFiles();

// Use the ForwardedHeaders middleware,
// updates the request's original IP address, scheme, and host based on the  X-Forwarded-For
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All
});

app.UseCors("CorsPolicy");


// User CORS policy
app.UseCors("CorsPolicy");

app.UseAuthorization();

//below the UseAuthorization part create a first middleware component
//app.Use(async (context, next) =>
//{
//    await context.Response.WriteAsync("Hello from the middleware component.");
//    // Call the next delegate/middleware in the pipeline
//    await next();
//});
//app.Run(async context =>
//{
//    await context.Response.WriteAsync("Hello from the 1nd delegate.");
//});
//app.Use(async (context, next) =>
//{
//    Console.WriteLine($"Logic before executing the next delegate in the Use method");
//    await next.Invoke();
//    Console.WriteLine($"Logic after executing the next delegate in the Use method");
//});
//app.Map("/usingmapbranch", builder =>
//{
//    builder.Use(async (context, next) =>
//    {
//        Console.WriteLine("Map branch logic in the Use method before the next delegate");
//        await next.Invoke();
//        Console.WriteLine("Map branch logic in the Use method after the next delegate");
//    });
//    builder.Run(async context =>
//    {
//        Console.WriteLine($"Map branch response to the client in the Run method");
//        await context.Response.WriteAsync("Hello from the map branch.");
//    });
//});
//app.MapWhen(context => context.Request.Query.ContainsKey("testquerystring"), builder =>
//{
//    builder.Run(async context =>
//    {
//        await context.Response.WriteAsync("Hello from the --MapWhen-- branch.");
//    });
//});

//app.Run(async context =>
//{
//    Console.WriteLine($"Writing the response to the client in the Run method");
//    await context.Response.WriteAsync("Hello from the middleware component.");
//});

app.MapControllers();

app.Run();
