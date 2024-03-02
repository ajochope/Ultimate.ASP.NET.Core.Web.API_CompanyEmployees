using CompanyEmployees.Extensions;
using Microsoft.AspNetCore.HttpOverrides;
using System.ComponentModel;

var builder = WebApplication.CreateBuilder(args);

// Use the ConfigureCors extension method
builder.Services.ConfigureCors();

// IIS integration
builder.Services.ConfigureIISIntegration();

// Add services to the container.

builder.Services.AddControllers();


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
app.Use(async (context, next) =>
{
    Console.WriteLine($"Logic before executing the next delegate in the Use method");
    await next.Invoke();
    Console.WriteLine($"Logic after executing the next delegate in the Use method");
});
app.Run(async context =>
{
    Console.WriteLine($"Writing the response to the client in the Run method");
    await context.Response.WriteAsync("Hello from the middleware component.");
});

app.MapControllers();

app.Run();
