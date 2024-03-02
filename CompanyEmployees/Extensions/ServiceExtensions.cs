namespace CompanyEmployees.Extensions
{
    public static class ServiceExtensions
    {
        //  CORS services to the DI container
        public static void ConfigureCors(this IServiceCollection services) =>
         services.AddCors(options =>
         {
             options.AddPolicy("CorsPolicy", builder =>
             builder.AllowAnyOrigin()
             .AllowAnyMethod()
             .AllowAnyHeader());
         });

    }
}
