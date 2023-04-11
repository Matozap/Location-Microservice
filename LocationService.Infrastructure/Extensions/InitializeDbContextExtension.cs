using System;
using System.Diagnostics.CodeAnalysis;
using LocationService.Infrastructure.Database.Context;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LocationService.Infrastructure.Extensions;

[ExcludeFromCodeCoverage]
public static class InitializeDbContextExtension
{
    public static void EnsureDatabaseIsSeeded(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        var databaseOptions = serviceProvider.GetService<DatabaseOptions>();
        var logger = serviceProvider.GetService<ILogger<DatabaseContext>>();
        using var serviceScope = serviceProvider.GetService<DatabaseContext>();
        
        try
        {
            if (databaseOptions.EngineType != EngineType.NonRelational)
            {
                if (!serviceScope.Database.CanConnect() || !serviceScope.Database.EnsureCreated())
                {
                    logger.LogWarning("[Database Initializer] Cannot connect to database {Database}", databaseOptions.DatabaseType);
                    return;
                }
            }

            if (!databaseOptions.SeedData) return;
            logger.LogInformation("[Database Initializer] Seeding starting");
            serviceScope.SeedData();
            logger.LogInformation("[Database Initializer] Seeding completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Database Initializer] Error Seeding database - {Error}", ex.Message);
            throw;
        }
    }
}
