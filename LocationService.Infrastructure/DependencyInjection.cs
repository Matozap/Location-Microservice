using System.Reflection;
using LocationService.Application.Interfaces;
using LocationService.Infrastructure.Caching;
using LocationService.Infrastructure.Database.Context;
using LocationService.Infrastructure.Database.Repositories;
using LocationService.Infrastructure.Extensions;
using MapsterMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LocationService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var cacheOptions= new CacheOptions();
        configuration.GetSection("Cache").Bind(cacheOptions);
        var databaseOptions = new DatabaseOptions();
        configuration.GetSection("Database").Bind(databaseOptions);

        services.AddDataContext(databaseOptions)
            .AddCache(cacheOptions)
            .AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly(), 
                typeof(DependencyInjection).Assembly, typeof(LocationService.Application.DependencyInjection).Assembly, typeof(IMapper).Assembly))
            .EnsureDatabaseIsSeeded();

        return services;
    }

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseWebApiExceptionHandler(env);            
        return app;
    }

    private static IServiceCollection AddDataContext(this IServiceCollection services, DatabaseOptions databaseOptions)
    {
        switch (databaseOptions.DatabaseType)
        {
            case "SQL Server":
            {
                services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(databaseOptions.ConnectionString));
                break;
            }
            
            case "MySql":
            {
                var serverVersion = ServerVersion.AutoDetect(databaseOptions.ConnectionString);

                services.AddDbContext<DatabaseContext>(
                    dbContextOptions => dbContextOptions
                        .UseMySql(databaseOptions.ConnectionString, serverVersion)
                        .EnableSensitiveDataLogging()
                        .EnableDetailedErrors()
                );
                
                break;
            }
            
            case "Postgres":
            {
                services.AddDbContext<DatabaseContext>(options => options.UseNpgsql(databaseOptions.ConnectionString));
                break;
            }
            
            case "Cosmos":
            {
                services.AddDbContext<DatabaseContext>(options => options.UseCosmos(databaseOptions.ConnectionString, databaseOptions.InstanceName, opt =>
                {
                    opt.ConnectionMode(ConnectionMode.Direct);
                    opt.ContentResponseOnWriteEnabled(false);
                }));
                break;
            }
            
            default:
            {
                services.AddDbContext<DatabaseContext>(options => options.UseInMemoryDatabase(databaseName: databaseOptions.InstanceName));
                break;
            }
        }
        
        services.AddSingleton(databaseOptions);
        services.AddScoped<ILocationRepository, LocationRepository>();
        return services;
    }

    private static IServiceCollection AddCache(this IServiceCollection services, CacheOptions cacheOptions)
    {
        switch (cacheOptions.CacheType)
        {
            case "Redis":
            {
                services.AddStackExchangeRedisCache(option =>
                {
                    option.Configuration = cacheOptions.ConnectionString;
                    option.InstanceName = cacheOptions.InstanceName;
                });
                break;
            }
            default:
            {
                services.AddDistributedMemoryCache();
                break;
            }
        }
        
        services.AddSingleton(cacheOptions);
        services.AddSingleton<ICache, Cache>();
        return services;
    }
}
