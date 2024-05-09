using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace YourNamespace
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();

    // Add CORS services
    services.AddCors(options =>
    {
        options.AddPolicy("AllowSpecificOrigin",
            builder =>
            {
                builder.WithOrigins("http://www.kaggle.com") // Replace with your origin
                       .AllowAnyHeader()
                       .AllowAnyMethod();
            });
    });

    // Add logging configuration
    services.AddLogging(logging =>
    {
        logging.AddConsole();
        logging.AddDebug();
    });

    // Add other services
    // ...
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseRouting();

    // Use CORS policy
    app.UseCors("AllowSpecificOrigin");

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });
}
  }
}