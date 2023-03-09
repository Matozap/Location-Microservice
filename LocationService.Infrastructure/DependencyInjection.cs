using System;
using System.Reflection;
using LocationService.Application.Interfaces;
using LocationService.Application.Logic.Countries.Queries.v1;
using LocationService.Infrastructure.Caching;
using LocationService.Infrastructure.Database.Context;
using LocationService.Infrastructure.Database.Repositories;
using LocationService.Infrastructure.Extensions;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LocationService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
        var isProduction = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true" && !isDevelopment;
        
        var cacheOptions= new CacheOptions();
        configuration.GetSection("Cache").Bind(cacheOptions);
        var databaseOptions = new DatabaseOptions();
        configuration.GetSection("Database").Bind(databaseOptions);

        services.AddDataContext(databaseOptions)
            .AddCache(cacheOptions)
            .AddMediatR(Assembly.GetExecutingAssembly().GetType(), typeof(ILocationRepository), typeof(GetAllCountriesHandler), typeof(IMapper))
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
            
            default:
            {
                services.AddDbContext<DatabaseContext>(options => options.UseInMemoryDatabase(databaseName: databaseOptions.InstanceName));
                break;
            }
        }
        
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
