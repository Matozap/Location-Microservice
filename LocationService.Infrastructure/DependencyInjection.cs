using System.Reflection;
using LocationService.Application.Interfaces;
using LocationService.Infrastructure.Database.Context;
using LocationService.Infrastructure.Database.Repositories;
using LocationService.Infrastructure.Extensions;
using LocationService.Message.Common;
using MapsterMapper;
using MediatrBuilder;
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
        var databaseOptions = new DatabaseOptions();
        configuration.GetSection("Database").Bind(databaseOptions);

        services.AddDataContext(databaseOptions)
            .AddMediatrBuilder(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly(), 
                typeof(DependencyInjection).Assembly, typeof(Application.DependencyInjection).Assembly, typeof(StringWrapper).Assembly, typeof(IMapper).Assembly)
                .AddFluentValidation(true))
            .EnsureDatabaseIsSeeded();

        return services;
    }

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app, IWebHostEnvironment env)
    {
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
        services.AddScoped<IRepository, EfRepository>();
        return services;
    }
}
