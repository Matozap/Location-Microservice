using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentValidation;
using LocationService.Application.Events.Publishers;
using LocationService.Application.Pipeline;
using LocationService.Domain;
using LocationService.Message.Contracts.Cities.v1;
using LocationService.Message.Contracts.Countries.v1;
using LocationService.Message.Contracts.States.v1;
using Mapster;
using MediatR;
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
            .Map(dest => dest.States, src => MapToStateData(src), src => src.States != null)
            .TwoWays()
            .IgnoreNullValues(true);
        TypeAdapterConfig<State, StateData>
            .NewConfig()
            .Map(dest => dest.Cities, src => MapToCityData(src))
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

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddScoped<IOutboxPublisher, OutboxPublisher>();
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ApplicationPipelineBehaviour<,>));
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
            Cities =  MapToCityData(state).ToList()
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
