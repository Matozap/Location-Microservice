using System.IO.Compression;
using System.Net;
using System.Text.Json.Serialization;
using LocationService.Application;
using LocationService.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = CreateFunctionHost();
host.Run();

IHost CreateFunctionHost() =>
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
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.DefaultConnectionLimit = 10000;
            
                var configuration = appBuilder.Configuration;
                services.AddCors();
                services.AddHealthChecks();
                services.AddControllers();
                services.AddMvc();
                services.AddApplication()
                    .AddInfrastructure(configuration)
                    .AddControllers()
                    .AddJsonOptions(x => x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);
                services.AddResponseCompression();

                services.Configure<GzipCompressionProviderOptions>
                (options => 
                { 
                    options.Level = CompressionLevel.Fastest; 
                }); 
                services.AddApplicationInsightsTelemetryWorkerService();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Startup] ERROR at ConfigureServices {ex}");
            }
            Console.WriteLine("[Startup] ConfigureServices [DONE]");
        })
        
        .Build();