using System.Diagnostics.CodeAnalysis;
using LocationService.Domain;
using LocationService.Message.DataTransfer.Cities.v1;
using LocationService.Message.DataTransfer.Countries.v1;
using LocationService.Message.DataTransfer.States.v1;
using Mapster;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace LocationService.Application;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        TypeAdapterConfig<Country, CountryData>
            .NewConfig()
            .IgnoreNullValues(true);
        TypeAdapterConfig<State, StateData>
            .NewConfig()
            .IgnoreNullValues(true);
        TypeAdapterConfig<City, CityData>
            .NewConfig()
            .IgnoreNullValues(true);
            
        return services;
    }

    public static IApplicationBuilder UseApplication(this IApplicationBuilder app)
    {
        return app;
    }
}
