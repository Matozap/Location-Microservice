using System;
using System.Net;
using System.Threading.Tasks;
using LocationService.API.Helpers;
using LocationService.API.Helpers.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace LocationService.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        await RunServer();
    }

    private static async Task RunServer()
    {
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        ServicePointManager.DefaultConnectionLimit = 10000;
        
        Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger(); 
        
        try
        {
            Log.Information("[Program] Host starting...");
            var isFunctionRuntime = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("FUNCTIONS_WORKER_RUNTIME"));
            
            var delay = int.Parse(Environment.GetEnvironmentVariable("DELAYED_START") ?? "0");
            Log.Information("[Program] Starting delay: {Delay} ms", delay);
            await Task.Delay(delay);
            
            var host = isFunctionRuntime ? CreateFunctionHost() : CreateKestrelHostBuilder();
            await host.RunAsync();

            Log.Information("[Program] Host stopped successfully");
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

    private static WebApplication CreateKestrelHostBuilder()
    {
        var builder = WebApplication.CreateBuilder();
        var environment = builder.Environment;
        var configuration = environment.GetEnvironmentConfiguration();
        
        builder.Services.AddWebApplicationServices(configuration);
        builder.Host.CreateLogger();
        builder.WebHost.UseKestrel(options =>
        {
            options.ListenAnyIP(5000);
            options.ListenAnyIP(5001, configure => configure.UseHttps());
        });
        builder.WebHost.UseKestrel();

        var app = builder.Build();
        app.AddHostMiddleware(environment, configuration);
        Log.Information("[Program] Host created successfully");
            
        return app;
    }

    private static IHost CreateFunctionHost() =>
        new HostBuilder()
            .ConfigureFunctionsWorkerDefaults(
                builder =>
                {
                    builder.UseMiddleware<GlobalExceptionMiddleware>();
                }
            )
            .ConfigureAppConfiguration((hostingContext, configBuilder) =>
            {
                var env = hostingContext.HostingEnvironment;
                configBuilder
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
            })
            .ConfigureServices((appBuilder, services) =>
            {
                try
                {
                    var configuration = appBuilder.Configuration;
                    services.AddFunctionServices(configuration);
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex, "[Program] Error creating function host - {Error}", ex.Message);
                }
                Log.Information("[Program] Configure services completed successfully");
            })
            .CreateLogger()
            .Build();
}
