using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using LocationService.Infrastructure.Database.Context;

namespace LocationService.Infrastructure.Extensions;

[ExcludeFromCodeCoverage]
public static class InitializeDbContextExtension
{
    public static void EnsureDatabaseIsSeeded(this IServiceCollection services)
    {
        try
        {
            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            using var serviceScope = serviceProvider.GetService<DatabaseContext>();
        }
        catch (Exception ex)
        {
            Console.WriteLine("[EnsureDatabaseIsSeeded] - " + ex.Message + " - " + ex.StackTrace);
        }
    }
}
