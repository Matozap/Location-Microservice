using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoFixture;
using Bustr.Bus;
using DistributedCache.Core;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Countries;
using LocationService.Application.Countries.Commands;
using LocationService.Application.Countries.Events;
using LocationService.Application.Countries.Queries;
using LocationService.Application.Countries.Requests;
using LocationService.Application.Countries.Responses;
using LocationService.Domain;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace LocationService.Test.MockBuilder;

public static class CountryMockBuilder
{
    private static readonly Fixture Fixture = new();

    private static IRepository GenerateMockRepository(Country location = null, int rowCount = 100)
    {
        var mockCountry = location ?? GenerateMockCountry();
        var mockCountiesList = GenerateMockDomainCountryList(rowCount);
        
        var repository = Substitute.For<IRepository>();
        
        repository.AddAsync(Arg.Any<Country>()).Returns(mockCountry);
        repository.UpdateAsync(mockCountry).Returns(mockCountry);
        repository.DeleteAsync(mockCountry).Returns(mockCountry);
        
        repository.GetAsSingleAsync(Arg.Any<Expression<Func<Country, bool>>>(), Arg.Any<Expression<Func<Country, string>>>(), 
            Arg.Any<Expression<Func<Country, string>>>(), Arg.Any<Expression<Func<Country, Country>>>(), Arg.Any<bool>()).Returns(mockCountry);
        repository.GetAsListAsync(Arg.Any<Expression<Func<Country, bool>>>(), Arg.Any<Expression<Func<Country, string>>>(), 
            Arg.Any<Expression<Func<Country, string>>>(), Arg.Any<Expression<Func<Country, Country>>>(), Arg.Any<bool>()).Returns(mockCountiesList);
        return repository;
    }

    private static ICache GenerateMockObjectCache()
    {
        var cache = Substitute.For<ICache>();
        return cache;
    }
    
    private static IEventBus GenerateMockEventBus()
    {
        var eventBus = Substitute.For<IEventBus>();
        eventBus.PublishAsync(Arg.Any<CountryEvent>()).Returns(Task.CompletedTask);
        return eventBus;
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
                GenerateMockRepository(location), GenerateMockObjectCache(), GenerateMockEventBus());
        }
        
        if (typeof(T) == typeof(SoftDeleteCountryHandler))
        {
            return new SoftDeleteCountryHandler(NullLogger<SoftDeleteCountryHandler>.Instance,
                GenerateMockRepository(location), GenerateMockObjectCache(),GenerateMockEventBus());
        }
        
        if (typeof(T) == typeof(DeleteCountryHandler))
        {
            return new DeleteCountryHandler(NullLogger<DeleteCountryHandler>.Instance,
                GenerateMockRepository(location), GenerateMockObjectCache(), GenerateMockEventBus());
        }
        
        if (typeof(T) == typeof(CreateCountryHandler))
        {
            var repository = GenerateMockRepository(location);
            repository.GetAsSingleAsync(Arg.Any<Expression<Func<Country, bool>>>(), Arg.Any<Expression<Func<Country, string>>>(), 
                Arg.Any<Expression<Func<Country, string>>>(), Arg.Any<Expression<Func<Country, Country>>>(), Arg.Any<bool>()).Returns((Country)null);
            return new CreateCountryHandler(NullLogger<CreateCountryHandler>.Instance,
                repository, GenerateMockObjectCache(), GenerateMockEventBus()) ;
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
