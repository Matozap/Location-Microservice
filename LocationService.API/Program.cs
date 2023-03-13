using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Threading.Tasks;


namespace LocationService.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        await RunServer();
    }

    private static async Task RunServer()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger(); 
        Log.Information("[Program] Starting Main");

        try
        {
            Log.Information("[Program] Host starting...");

            if (int.TryParse(Environment.GetEnvironmentVariable("DELAYED_START"), out var delay))
            {
                Log.Information("[Program] Starting delay: {Delay}", delay);
                await Task.Delay(delay);
            }

            await CreateHostBuilder(null).Build().RunAsync();
            Log.Information("[Program] Host Stopped Successfully");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>()
                    .CaptureStartupErrors(true);
            })
            .UseSerilog((hostingContext, loggerConfiguration) => {
                loggerConfiguration
                    .ReadFrom.Configuration(hostingContext.Configuration)
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("ApplicationName", typeof(Program).Assembly.GetName().Name)
                    .Enrich.WithProperty("Environment", hostingContext.HostingEnvironment);
            });
}
