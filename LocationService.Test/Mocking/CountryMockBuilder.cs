using System;
using AutoFixture;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Domain;
using LocationService.Message.DTO.Countries.v1;
using LocationService.Message.Messaging.Response.Countries.v1;

namespace LocationService.Test.Mocking;

public static class CountryMockBuilder
{
    private static readonly Fixture Fixture = new();

    public static ILocationRepository GenerateMockRepository(Country location = null, int rowCount = 100)
    {
        var mockCountry = location ?? GenerateMockCountry();
        var mockCounties = GenerateMockCountryList(rowCount);
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

    private static List<Country> GenerateMockCountryList(int count)
    {
        return Fixture.Build<Country>()
            .Without(s => s.States)
            .CreateMany(count)
            .ToList();
    }

    public static List<CountryData> GenerateMockCountryDtoList(int count)
    {
        return Fixture.Build<CountryData>().CreateMany(count).ToList();
    }

    public static Country GenerateMockCountry()
    {
        return Fixture.Build<Country>()
            .Create();
    }

    public static CountryFlatData GenerateMockCountryDto()
    {
        return Fixture.Build<CountryFlatData>().Create();
    }

    public static CountryData GenerateMockCountryDto(Country location)
    {
        return Fixture.Build<CountryData>()
            .Create();
    }
}
