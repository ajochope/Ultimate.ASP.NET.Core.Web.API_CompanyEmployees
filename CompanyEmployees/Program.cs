using CompanyEmployees.Extensions;
using Contracts;
using LoggerService;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;
using NLog;

var builder = WebApplication.CreateBuilder(args);

// configuration for a logger service
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var configFileName = $"nlog.{environment}.config";
var configFilePath = Path.Combine(Directory.GetCurrentDirectory(), configFileName);

LogManager.Setup().LoadConfigurationFromFile(configFilePath);

// configures support for JSON Patch using Newtonsoft.Json
NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter() =>
    new ServiceCollection().AddLogging().AddMvc().AddNewtonsoftJson()
        .Services.BuildServiceProvider()
        .GetRequiredService<IOptions<MvcOptions>>().Value.InputFormatters
        .OfType<NewtonsoftJsonPatchInputFormatter>().First();

// Use the ConfigureCors extension method
builder.Services.ConfigureCors();

// IIS integration
builder.Services.ConfigureIISIntegration();

// Logger service
builder.Services.ConfigureLoggerService();

//  Validation to enable our custom responses from the actions
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// Add services to the container.
builder.Services.AddControllers()
                .AddApplicationPart(typeof(CompanyEmployees.Presentation.AssemblyReference).Assembly);

// Repository pattern separating the logic from model
builder.Services.ConfigureRepositoryManager();

// Encapsulates the registration of all service classes togehter in a service manager
builder.Services.ConfigureServiceManager();

// Register AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Enable the server to format the XML
builder.Services.AddControllers(config => {
            config.RespectBrowserAcceptHeader = true;
            config.ReturnHttpNotAcceptable = true;
            config.InputFormatters.Insert(0, GetJsonPatchInputFormatter());
}).AddXmlDataContractSerializerFormatters()
          .AddCustomCSVFormatter()
          .AddApplicationPart(typeof(CompanyEmployees.Presentation.AssemblyReference).Assembly);

// SQL server connection
builder.Services.ConfigureSqlContext(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.

// Configure an exception handler
var logger = app.Services.GetRequiredService<ILoggerManager>();
app.ConfigureExceptionHandler(logger);

//handling exceptions and HTTP Strict Transport Security (HSTS)
if (app.Environment.IsProduction())
    app.UseHsts();

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
