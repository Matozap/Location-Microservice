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
            logger.LogInformation("Database Type: {DatabaseType}", databaseOptions.DatabaseType);
            if (databaseOptions.EngineType != EngineType.NonRelational)
            {
                if (!serviceScope.Database.CanConnect() || !serviceScope.Database.EnsureCreated())
                {
                    logger.LogInformation("[Database Initializer] Connection test to database {Database} was inconclusive", databaseOptions.DatabaseType);
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
