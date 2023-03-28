using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace LocationService.API.Helpers;

[ExcludeFromCodeCoverage]
public static class EnvironmentConfigurationExtension
{
    private const string RootFolderName = "bootstrap";
    public static IConfiguration GetEnvironmentConfiguration(this IConfiguration configuration, IWebHostEnvironment env)
    {
        try
        {
            var basePath = env.ContentRootPath;
            if (!File.Exists($"{basePath}/appsettings.json"))
            {
                basePath = $"{env.ContentRootPath}/{RootFolderName}";
                var jsonFile = $"{basePath}/appsettings.json";
                if (!File.Exists(jsonFile))
                {
                    throw new Exception($"[ConfigurationBuilder] Original Path and fallback path ({jsonFile}) is not valid or does not exists. {GetFiles(basePath)}");
                }
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

    private static string GetFiles(string path)
    {
        var filePaths = Directory.GetFiles(path, "*.*",
            SearchOption.TopDirectoryOnly);
        var files = "";
        files = filePaths.Aggregate(files, (current, item) => current + item);

        return files;
    }
}
