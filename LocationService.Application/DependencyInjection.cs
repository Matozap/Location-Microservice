using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
            .Map(dest => dest.Details, src => new StateCollection { States = 
            {
                MapToStateData(src)
            }}, src => src.States != null)
            .TwoWays()
            .IgnoreNullValues(true);
        TypeAdapterConfig<State, StateData>
            .NewConfig()
            .Map(dest => dest.Details, src => new CityCollection { Cities =
            {
                MapToCityData(src)
            }})
            .IgnoreNullValues(true);
        TypeAdapterConfig<StateData, State>
            .NewConfig()
            .Map(dest => dest.Country, src => (Country)null)
            .IgnoreNullValues(true);
        TypeAdapterConfig<City, CityData>
            .NewConfig()
            .IgnoreNullValues(true);
        TypeAdapterConfig<CityData, City>
            .NewConfig()
            .Map(dest => dest.State, src => (State)null)
            .IgnoreNullValues(true);
            
        return services;
    }

    private static IEnumerable<StateData> MapToStateData(Country src)
    {
        return src.States.Select(state => new StateData
        {
            Id = state.Id,
            Code = state.Code,
            Name = state.Name,
            CountryId = state.CountryId,
            Details = state.Cities != null ? new CityCollection { Cities =
            {
                MapToCityData(state)
            }} : new CityCollection()
        });
    }

    private static IEnumerable<CityData> MapToCityData(State src)
    {
        return src.Cities.Select(city => new CityData
        {
            Id = city.Id,
            Name = city.Name,
            StateId = city.StateId
        });
    }

    public static IApplicationBuilder UseApplication(this IApplicationBuilder app)
    {
        return app;
    }
}
