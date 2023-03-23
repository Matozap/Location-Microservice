using System.IO.Compression;
using System.Net;
using System.Text.Json.Serialization;
using LocationService.Application;
using LocationService.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LocationService.API.Helpers;

public static class StartupServicesExtension
{
    public static void AddStartupServicesForControllers(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthorization();
        services.AddHealthChecks();
        services.AddControllers()
            .AddJsonOptions(x => x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);
        services.AddMvc();
        services.ConfigureSwagger();
        services.AddApplicationInsightsTelemetry();
        AddSharedStartupServices(services, configuration);
    }
    
    public static void AddStartupServicesForFunctions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        AddSharedStartupServices(services, configuration);
    }

    private static void AddSharedStartupServices(IServiceCollection services, IConfiguration configuration)
    {
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        ServicePointManager.DefaultConnectionLimit = 10000;

        services.AddCors();
        services.AddApplication()
            .AddInfrastructure(configuration)
            .AddEventBus(configuration);
        services.AddResponseCompression();

        services.Configure<GzipCompressionProviderOptions>
        (options => 
        { 
            options.Level = CompressionLevel.Fastest; 
        }); 
    }
}