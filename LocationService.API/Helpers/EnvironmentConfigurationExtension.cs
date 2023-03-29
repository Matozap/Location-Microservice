using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace LocationService.API.Helpers;

[ExcludeFromCodeCoverage]
public static class EnvironmentConfigurationExtension
{
    public static IConfiguration GetEnvironmentConfiguration(this IConfiguration configuration, IWebHostEnvironment env)
    {
        try
        {
            var basePath = env.ContentRootPath;
            if (!File.Exists($"{basePath}/appsettings.json"))
            {
                throw new ConfigurationErrorsException("appsettings.json file is missing from the project output and it is required for it to work");
            }

            Console.WriteLine($"[ConfigurationBuilder] Environment: {env.EnvironmentName}");
            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            builder.AddEnvironmentVariables();
            return builder.Build();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ConfigurationBuilder] Error: {ex.Message} - {ex.StackTrace}");
            return configuration;
        }
    }
}
