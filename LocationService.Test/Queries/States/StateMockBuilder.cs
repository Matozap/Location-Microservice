using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoFixture;
using LocationService.Application.Commands.States.v1;
using LocationService.Application.Interfaces;
using LocationService.Application.Queries.States.v1;
using LocationService.Domain;
using LocationService.Message.DTO.States.v1;
using LocationService.Message.Messaging.Request.States.v1;
using LocationService.Message.Messaging.Response.States.v1;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace LocationService.Test.Queries.States;

public static class StateMockBuilder
{
    private static readonly Fixture Fixture = new();

    private static ILocationRepository GenerateMockRepository(State location = null, int rowCount = 100)
    {
        var mockState = location ?? GenerateMockState();
        var mockCounties = GenerateMockDomainStateList(rowCount);
        
        var repository = Substitute.For<ILocationRepository>();
        
        repository.AddAsync(Arg.Any<State>()).Returns(mockState);
        repository.UpdateAsync(mockState).Returns(mockState);
        repository.DeleteAsync(mockState).Returns(mockState);
        
        repository.GetAllStatesAsync(Arg.Any<string>()).Returns(mockCounties);
        repository.GetStateAsync(Arg.Any<Expression<Func<State, bool>>>()).Returns(mockState);
        return repository;
    }

    private static IEventBus GenerateMockEventBus()
    {
        var eventBus = Substitute.For<IEventBus>();
        eventBus.Publish(Arg.Any<StateCreated>()).Returns(Task.CompletedTask);
        return eventBus;
    }

    private static IObjectCache GenerateMockObjectCache()
    {
        var cache = Substitute.For<IObjectCache>();
        return cache;
    }

    private static List<State> GenerateMockDomainStateList(int count)
    {
        return Fixture.Build<State>()
            .Without(s => s.Country)
            .Without(s => s.Cities)
            .CreateMany(count)
            .ToList();
    }

    public static List<StateData> GenerateMockStateDtoList(int count)
    {
        return Fixture.Build<StateData>()
            .Without(s => s.Cities)
            .CreateMany(count).ToList();
    }

    public static State GenerateMockState()
    {
        return Fixture.Build<State>()
            .Without(s => s.Country)
            .Without(s => s.Cities)
            .Create();
    }


    public static object CreateHandler<T>()
    {
        var response = GenerateMockStateDtoList(1).FirstOrDefault();
        var location = GenerateMockState();
        
        var mediator = Substitute.For<IMediator>();
        mediator.Send(Arg.Any<GetStateById>()).Returns(response);

        if (typeof(T) == typeof(UpdateStateHandler))
        {
            return new UpdateStateHandler(NullLogger<UpdateStateHandler>.Instance,
                GenerateMockRepository(location),
                mediator,
                GenerateMockEventBus());
        }
        
        if (typeof(T) == typeof(SoftDeleteStateHandler))
        {
            return new SoftDeleteStateHandler(NullLogger<SoftDeleteStateHandler>.Instance,
                GenerateMockRepository(location),
                mediator,
                GenerateMockEventBus());
        }
        
        if (typeof(T) == typeof(DeleteStateHandler))
        {
            return new DeleteStateHandler(NullLogger<DeleteStateHandler>.Instance,
                GenerateMockRepository(location),
                mediator,
                GenerateMockEventBus());
        }
        
        if (typeof(T) == typeof(CreateStateHandler))
        {
            return new CreateStateHandler(NullLogger<CreateStateHandler>.Instance,
                GenerateMockRepository(location),
                GenerateMockEventBus());
        }
        
        if (typeof(T) == typeof(GetAllStatesHandler))
        {
            return new GetAllStatesHandler(GenerateMockObjectCache(),
                NullLogger<GetAllStatesHandler>.Instance,
                GenerateMockRepository(rowCount: 10));
        }
        
        if (typeof(T) == typeof(GetStateByIdHandler))
        {
            return new GetStateByIdHandler(GenerateMockRepository(location),
                GenerateMockObjectCache(),
                NullLogger<GetStateByIdHandler>.Instance);
        }
        
        return null;
    } 
}
