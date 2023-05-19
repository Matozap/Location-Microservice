using System;
using System.Configuration;
using System.IO;
using Bustr.Bus;
using DistributedCache.Core.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace LocationService.API.Helpers.Configuration;

public static class ConfigurationHelper
{
    public record GrpcConfigurationSettings(bool Disabled, bool ReflectionDisabled);

    public class BusConfigurationSettings
    {
        public string Type { get; init; } 
        public string ConnectionString { get; init; } 
        public bool Disabled { get; init; } 
        public BusType BusType => Type switch
        {
            "Azure Service Bus" => BusType.AzureServiceBus,
            "RabbitMQ" => BusType.RabbitMq,
            _ => BusType.InMemory
        };
    }
    
    public class CacheConfigurationSettings
    {
        public string Type { get; init; } 
        public string ConnectionString { get; init; } 
        public string InstanceName { get; init; } 
        public bool Disabled { get; init; } 
        public CacheType CacheType => Type switch
        {
            "Redis" => CacheType.Redis,
            "Sql Server" => CacheType.SqlServer,
            _ => CacheType.InMemory
        };
    }
    
    public static GrpcConfigurationSettings GrpcSettings(IConfiguration configuration)
    {
        var settings = new GrpcConfigurationSettings(false, false);
        configuration.GetSection("Grpc").Bind(settings);
        return settings;
    }
    
    public static BusConfigurationSettings BusSettings(IConfiguration configuration)
    {
        var settings = new BusConfigurationSettings();
        configuration.GetSection("EventBus").Bind(settings);
        return settings;
    }
    
    public static CacheConfigurationSettings CacheSettings(IConfiguration configuration)
    {
        var settings = new CacheConfigurationSettings();
        configuration.GetSection("Cache").Bind(settings);
        return settings;
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
}