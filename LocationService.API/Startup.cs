using System;
using LocationService.API.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LocationService.Application;
using LocationService.Infrastructure;

namespace LocationService.API;

public class Startup
{
    private IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        Configuration = configuration.GetEnvironmentConfiguration(env);            
    }
                        
    public void ConfigureServices(IServiceCollection services)
    {
        try
        {
            services.AddStartupServicesForControllers(Configuration);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Startup] Error at ConfigureServices {ex}");
        }
        Console.WriteLine("[Startup] Services Configuration [DONE]");
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        try
        {                
            app.UseSwaggerApi();
                
            app.UseApplication()
                .UseInfrastructure(env)
                .UseHttpsRedirection()
                .UseRouting();

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/heartbeat");
                endpoints.MapControllers();
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Startup] Error at Configure - {ex}");
        }

        Console.WriteLine("[Startup] Application Configuration [DONE]");
    }
}
