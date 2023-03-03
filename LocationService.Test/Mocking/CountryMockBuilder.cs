using System;
using AutoFixture;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LocationService.Application.Commands.Countries.v1;
using LocationService.Application.Interfaces;
using LocationService.Application.Queries.Countries.v1;
using LocationService.Domain;
using LocationService.Message.DTO.Countries.v1;
using LocationService.Message.Messaging.Request.Countries.v1;
using LocationService.Message.Messaging.Response.Countries.v1;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;

namespace LocationService.Test.Mocking;

public static class CountryMockBuilder
{
    private static readonly Fixture Fixture = new();

    public static ILocationRepository GenerateMockRepository(Country location = null, int rowCount = 100)
    {
        var mockCountry = location ?? GenerateMockCountry();
        var mockCounties = GenerateMockDomainCountryList(rowCount);
        
        var repository = Substitute.For<ILocationRepository>();
        
        repository.AddAsync(Arg.Any<Country>()).Returns(mockCountry);
        repository.UpdateAsync(mockCountry).Returns(mockCountry);
        repository.DeleteAsync(mockCountry).Returns(mockCountry);
        
        repository.GetAllCountriesAsync().Returns(mockCounties);
        repository.GetCountryAsync(Arg.Any<Expression<Func<Country, bool>>>()).Returns(mockCountry);
        return repository;
    }

    public static IEventBus GenerateMockEventBus()
    {
        var eventBus = Substitute.For<IEventBus>();
        eventBus.Publish(Arg.Any<CountryCreated>()).Returns(Task.CompletedTask);
        return eventBus;
    }

    public static IObjectCache GenerateMockObjectCache()
    {
        var cache = Substitute.For<IObjectCache>();
        return cache;
    }

    private static List<Country> GenerateMockDomainCountryList(int count)
    {
        return Fixture.Build<Country>()
            .Without(s => s.States)
            .CreateMany(count)
            .ToList();
    }

    public static List<CountryData> GenerateMockCountryDtoList(int count)
    {
        return Fixture.Build<CountryData>()
            .Without(s => s.States)
            .CreateMany(count).ToList();
    }

    public static Country GenerateMockCountry()
    {
        return Fixture.Build<Country>()
            .Without(s => s.States)
            .Create();
    }


    public static object CreateHandler<T>()
    {
        var response = GenerateMockCountryDtoList(1).FirstOrDefault();
        var location = GenerateMockCountry();
        
        var mediator = Substitute.For<IMediator>();
        mediator.Send(Arg.Any<GetCountryById>()).Returns(response);

        if (typeof(T) == typeof(UpdateCountryHandler))
        {
            return new UpdateCountryHandler(NullLogger<UpdateCountryHandler>.Instance,
                GenerateMockRepository(location),
                mediator,
                GenerateMockEventBus());
        }
        
        if (typeof(T) == typeof(SoftDeleteCountryHandler))
        {
            return new SoftDeleteCountryHandler(NullLogger<SoftDeleteCountryHandler>.Instance,
                GenerateMockRepository(location),
                mediator,
                GenerateMockEventBus());
        }
        
        if (typeof(T) == typeof(DeleteCountryHandler))
        {
            return new DeleteCountryHandler(NullLogger<DeleteCountryHandler>.Instance,
                GenerateMockRepository(location),
                mediator,
                GenerateMockEventBus());
        }
        
        if (typeof(T) == typeof(CreateCountryHandler))
        {
            return new CreateCountryHandler(NullLogger<CreateCountryHandler>.Instance,
                GenerateMockRepository(location),
                GenerateMockEventBus());
        }
        
        if (typeof(T) == typeof(GetAllCountriesHandler))
        {
            return new GetAllCountriesHandler(GenerateMockObjectCache(),
                NullLogger<GetAllCountriesHandler>.Instance,
                GenerateMockRepository(rowCount: 10));
        }
        
        if (typeof(T) == typeof(GetCountryByIdHandler))
        {
            return new GetCountryByIdHandler(GenerateMockRepository(location),
                GenerateMockObjectCache(),
                NullLogger<GetCountryByIdHandler>.Instance);
        }
        
        return null;
    } 
}
