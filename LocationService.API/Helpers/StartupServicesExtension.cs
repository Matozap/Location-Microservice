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
        if(GrpcSettings(configuration).Disabled) return;
        services.AddCodeFirstGrpc(
            config => { config.ResponseCompressionLevel = CompressionLevel.Optimal; }
        );
        if(GrpcSettings(configuration).ReflectionDisabled) return;
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
        
        if(GrpcSettings(configuration).Disabled) return;
        app.UseGrpcWeb();
        app.MapGrpcService<CityService>().EnableGrpcWeb();
        app.MapGrpcService<StateService>().EnableGrpcWeb();
        app.MapGrpcService<CountryService>().EnableGrpcWeb();
        
        if(GrpcSettings(configuration).ReflectionDisabled) return;
        app.MapCodeFirstGrpcReflectionService().EnableGrpcWeb();
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