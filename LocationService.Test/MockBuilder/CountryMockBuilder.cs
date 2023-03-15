using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoFixture;
using LocationService.Application.Interfaces;
using LocationService.Application.Logic.Countries.Commands.v1;
using LocationService.Application.Logic.Countries.Queries.v1;
using LocationService.Domain;
using LocationService.Message.DataTransfer.Countries.v1;
using LocationService.Message.Definition.Countries.Requests.v1;
using LocationService.Message.Definition.Countries.Responses.v1;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace LocationService.Test.MockBuilder;

public static class CountryMockBuilder
{
    private static readonly Fixture Fixture = new();

    private static ILocationRepository GenerateMockRepository(Country location = null, int rowCount = 100)
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

    private static IEventBus GenerateMockEventBus()
    {
        var eventBus = Substitute.For<IEventBus>();
        eventBus.Publish(Arg.Any<CountryCreated>()).Returns(Task.CompletedTask);
        return eventBus;
    }

    private static ICache GenerateMockObjectCache()
    {
        var cache = Substitute.For<ICache>();
        return cache;
    }

    private static List<Country> GenerateMockDomainCountryList(int count)
    {
        return Fixture.Build<Country>()
            .Without(s => s.States)
            .With(s => s.LastUpdateDate, () => DateTime.Now)
            .With(s => s.LastUpdateUserId, () => "Test")
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
            .With(s => s.LastUpdateDate, () => DateTime.Now)
            .With(s => s.LastUpdateUserId, () => "Test")
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
                GenerateMockEventBus());
        }
        
        if (typeof(T) == typeof(SoftDeleteCountryHandler))
        {
            return new SoftDeleteCountryHandler(NullLogger<SoftDeleteCountryHandler>.Instance,
                GenerateMockRepository(location),
                GenerateMockEventBus());
        }
        
        if (typeof(T) == typeof(DeleteCountryHandler))
        {
            return new DeleteCountryHandler(NullLogger<DeleteCountryHandler>.Instance,
                GenerateMockRepository(location),
                GenerateMockEventBus());
        }
        
        if (typeof(T) == typeof(CreateCountryHandler))
        {
            var repository = GenerateMockRepository(location);
            repository.GetCountryAsync(Arg.Any<Expression<Func<Country, bool>>>()).Returns((Country)null);
            return new CreateCountryHandler(NullLogger<CreateCountryHandler>.Instance,
                repository,
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
