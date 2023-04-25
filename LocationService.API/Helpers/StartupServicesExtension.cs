using System;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;
using LocationService.API.Inputs.Grpc;
using LocationService.Application;
using LocationService.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProtoBuf.Grpc.Server;
using Serilog;

namespace LocationService.API.Helpers;

public static class StartupServicesExtension
{
    private record GrpcOptions(bool Disabled, bool ReflectionDisabled);
    public static void AddWebApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors();
        services.AddHealthChecks();
        services.AddControllers()
            .AddJsonOptions(x => x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);
        services.AddMvc();
        services.ConfigureSwagger();
        services.AddApplicationInsightsTelemetry();
        services.AddResponseCompression();
        services.Configure<GzipCompressionProviderOptions>
        (options => 
        { 
            options.Level = CompressionLevel.Fastest; 
        }); 
        
        AddDependencies(services, configuration);
        AddOptionalServices(services, configuration);
        services.AddTransient<GlobalExceptionMiddleware>();
    }
    
    public static void AddFunctionServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.Configure<JsonSerializerOptions>(options =>
        {
            options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.PropertyNameCaseInsensitive = true;
        });
        
        AddDependencies(services, configuration);
    }
    
    private static void AddOptionalServices(this IServiceCollection services, IConfiguration configuration)
    {
        var grpcSettings = GrpcSettings(configuration);
        if(grpcSettings.Disabled) return;
        services.AddCodeFirstGrpc(
            config =>
            {
                config.ResponseCompressionLevel = CompressionLevel.Optimal;
                config.EnableDetailedErrors = true;
                config.IgnoreUnknownServices = true;
                config.Interceptors.Add<GlobalExceptionMiddleware>();
            }
        );
        if(grpcSettings.ReflectionDisabled) return;
        services.AddCodeFirstGrpcReflection();
    }
    
    private static void AddDependencies(IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplication()
            .AddInfrastructure(configuration)
            .AddEventBus(configuration);
    }
    
    public static void AddHostMiddleware(this WebApplication app, IWebHostEnvironment environment, IConfiguration configuration)
    {
        app.UseSwaggerApi();
        app.UseApplication()
            .UseInfrastructure(environment)
            .UseRouting();
        app.UseCors(x => x
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
        app.MapHealthChecks("/heartbeat");
        app.MapControllers();
        app.UseMiddleware<GlobalExceptionMiddleware>();
        
        if(GrpcSettings(configuration).Disabled) return;
        app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true});
        app.MapGrpcService<CityService>();
        app.MapGrpcService<StateService>();
        app.MapGrpcService<CountryService>();
        
        if(GrpcSettings(configuration).ReflectionDisabled) return;
        app.MapCodeFirstGrpcReflectionService();
    }

    public static IHostBuilder CreateLogger(this IHostBuilder webHostBuilder)
    {
        return webHostBuilder.UseSerilog((hostingContext, loggerConfiguration) =>
        {
            loggerConfiguration
                .ReadFrom.Configuration(hostingContext.Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("ApplicationName", typeof(Program).Assembly.GetName().Name ?? "Application")
                .Enrich.WithProperty("Environment", hostingContext.HostingEnvironment);
        });
    }
    
    public static IConfiguration GetEnvironmentConfiguration(this IWebHostEnvironment env)
    {
        var basePath = env.ContentRootPath;
        if (!File.Exists($"{basePath}/appsettings.json"))
        {
            throw new ConfigurationErrorsException("appsettings.json file is missing from the project output and it is required");
        }

        Console.WriteLine($"Environment: {env.EnvironmentName}");
        var builder = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

        builder.AddEnvironmentVariables();
        return builder.Build();
    }

    private static GrpcOptions GrpcSettings(IConfiguration configuration)
    {
        var settings = new GrpcOptions(false, false);
        configuration.GetSection("Grpc").Bind(settings);
        return settings;
    }
}