using CompanyEmployees.Extensions;
using Microsoft.AspNetCore.HttpOverrides;

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

app.MapControllers();

app.Run();
