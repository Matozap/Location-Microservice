using System;
using System.Reflection;
using LocationService.Application.Interfaces;
using LocationService.Application.Queries.Countries.v1;
using LocationService.Infrastructure.Data.Cache;
using LocationService.Infrastructure.Data.Context;
using LocationService.Infrastructure.Data.Repository;
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

        services.AddDataContext(configuration, isProduction)
            .AddCache(configuration, isProduction)
            .AddMediatR(Assembly.GetExecutingAssembly().GetType(), typeof(ILocationRepository), typeof(GetAllCountriesHandler), typeof(IMapper))
            .EnsureDatabaseIsSeeded();

        return services;
    }

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseWebApiExceptionHandler(env);            
        return app;
    }

    private static IServiceCollection AddDataContext(this IServiceCollection services, IConfiguration configuration, bool isProduction)
    {
        var locationContextOptions = new LocationContextOptions();
        configuration.GetSection("LocationContext").Bind(locationContextOptions);
        
        switch (isProduction)
        {
            case true:
            {
                services.AddDbContext<LocationContext>(options => options.UseSqlServer(locationContextOptions.ConnectionString));
                break;
            }
            
            case false:
            {
                var database = locationContextOptions.Database ?? "Local";
                services.AddDbContext<LocationContext>(options => options.UseInMemoryDatabase(databaseName: database));
                break;
            }
        }
        
        services.AddScoped<ILocationRepository, LocationRepository>();
        return services;
    }

    private static IServiceCollection AddCache(this IServiceCollection services, IConfiguration configuration, bool isProduction)
    {
        var cacheOptions= new CacheOptions();
        configuration.GetSection("Cache").Bind(cacheOptions);
        
        switch (isProduction)
        {
            case true:
            {
                if (!cacheOptions.Disabled)
                {
                    services.AddStackExchangeRedisCache(option =>
                    {
                        option.Configuration = cacheOptions.ConnectionString;
                        option.InstanceName = "location";
                    });
                }
                break;
            }
            case false:
            {
                services.AddDistributedMemoryCache();
                break;
            }
        }
        
        services.AddSingleton(cacheOptions);
        services.AddSingleton<IObjectCache, ObjectCache>();
        return services;
    }
}
