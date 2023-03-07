using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoFixture;
using LocationService.Application.Commands.Cities.v1;
using LocationService.Application.Interfaces;
using LocationService.Application.Queries.Cities.v1;
using LocationService.Domain;
using LocationService.Message.DTO.Cities.v1;
using LocationService.Message.Messaging.Request.Cities.v1;
using LocationService.Message.Messaging.Response.Cities.v1;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace LocationService.Test.MockBuilder;

public static class CityMockBuilder
{
    private static readonly Fixture Fixture = new();

    private static ILocationRepository GenerateMockRepository(City location = null, int rowCount = 100)
    {
        var mockCity = location ?? GenerateMockCity();
        var mockCounties = GenerateMockDomainCityList(rowCount);
        
        var repository = Substitute.For<ILocationRepository>();
        
        repository.AddAsync(Arg.Any<City>()).Returns(mockCity);
        repository.UpdateAsync(mockCity).Returns(mockCity);
        repository.DeleteAsync(mockCity).Returns(mockCity);
        
        repository.GetAllCitiesAsync(Arg.Any<int>()).Returns(mockCounties);
        repository.GetCityAsync(Arg.Any<Expression<Func<City, bool>>>()).Returns(mockCity);
        return repository;
    }

    private static IEventBus GenerateMockEventBus()
    {
        var eventBus = Substitute.For<IEventBus>();
        eventBus.Publish(Arg.Any<CityCreated>()).Returns(Task.CompletedTask);
        return eventBus;
    }

    private static IObjectCache GenerateMockObjectCache()
    {
        var cache = Substitute.For<IObjectCache>();
        return cache;
    }

    private static List<City> GenerateMockDomainCityList(int count)
    {
        return Fixture.Build<City>()
            .Without(s => s.State)
            .With(s => s.LastUpdateDate, () => DateTime.Now)
            .With(s => s.LastUpdateUserId, () => "Test")
            .CreateMany(count)
            .ToList();
    }

    public static List<CityData> GenerateMockCityDtoList(int count)
    {
        return Fixture.Build<CityData>()
            .CreateMany(count).ToList();
    }

    public static City GenerateMockCity()
    {
        return Fixture.Build<City>()
            .Without(s => s.State)
            .With(s => s.LastUpdateDate, () => DateTime.Now)
            .With(s => s.LastUpdateUserId, () => "Test")
            .Create();
    }


    public static object CreateHandler<T>()
    {
        var response = GenerateMockCityDtoList(1).FirstOrDefault();
        var location = GenerateMockCity();
        
        var mediator = Substitute.For<IMediator>();
        mediator.Send(Arg.Any<GetCityById>()).Returns(response);

        if (typeof(T) == typeof(UpdateCityHandler))
        {
            return new UpdateCityHandler(NullLogger<UpdateCityHandler>.Instance,
                GenerateMockRepository(location),
                mediator,
                GenerateMockEventBus());
        }
        
        if (typeof(T) == typeof(SoftDeleteCityHandler))
        {
            return new SoftDeleteCityHandler(NullLogger<SoftDeleteCityHandler>.Instance,
                GenerateMockRepository(location),
                mediator,
                GenerateMockEventBus());
        }
        
        if (typeof(T) == typeof(DeleteCityHandler))
        {
            return new DeleteCityHandler(NullLogger<DeleteCityHandler>.Instance,
                GenerateMockRepository(location),
                mediator,
                GenerateMockEventBus());
        }
        
        if (typeof(T) == typeof(CreateCityHandler))
        {
            return new CreateCityHandler(NullLogger<CreateCityHandler>.Instance,
                GenerateMockRepository(location),
                GenerateMockEventBus());
        }
        
        if (typeof(T) == typeof(GetAllCitiesHandler))
        {
            return new GetAllCitiesHandler(GenerateMockObjectCache(),
                NullLogger<GetAllCitiesHandler>.Instance,
                GenerateMockRepository(rowCount: 10));
        }
        
        if (typeof(T) == typeof(GetCityByIdHandler))
        {
            return new GetCityByIdHandler(GenerateMockRepository(location),
                GenerateMockObjectCache(),
                NullLogger<GetCityByIdHandler>.Instance);
        }
        
        return null;
    } 
}
