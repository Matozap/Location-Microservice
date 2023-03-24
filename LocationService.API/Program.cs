using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Threading.Tasks;
using LocationService.API.Helpers;
using Microsoft.Extensions.Configuration;


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

            var isFunctionRuntime = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("FUNCTIONS_WORKER_RUNTIME"));

            if (isFunctionRuntime)
            {
                await CreateFunctionHost().RunAsync();
            }
            else
            {
                if (int.TryParse(Environment.GetEnvironmentVariable("DELAYED_START"), out var delay))
                {
                    Log.Information("[Program] Starting delay: {Delay}", delay);
                    await Task.Delay(delay);
                }

                await CreateHostBuilder(null).Build().RunAsync();
            }

            Log.Information("[Program] Host Stopped Successfully");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "[Program] Host terminated unexpectedly");
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
            .CreateLogger();
    
    private static IHost CreateFunctionHost() =>
        new HostBuilder()
            .ConfigureFunctionsWorkerDefaults()
            .ConfigureAppConfiguration((hostingContext, configBuilder) =>
            {
                var env = hostingContext.HostingEnvironment;
                configBuilder
                    .AddJsonFile($"appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
            })
            .ConfigureServices((appBuilder, services) =>
            {
                try
                {
                    var configuration = appBuilder.Configuration;
                    services.AddStartupServicesForFunctions(configuration);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Startup] ERROR at ConfigureServices {ex}");
                }
                Console.WriteLine("[Startup] ConfigureServices [DONE]");
            })
            .CreateLogger()
            .Build();
}
