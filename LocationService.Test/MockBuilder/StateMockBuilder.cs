using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoFixture;
using LocationService.Application.Interfaces;
using LocationService.Application.Logic.States.Commands.v1;
using LocationService.Application.Logic.States.Queries.v1;
using LocationService.Domain;
using LocationService.Message.DataTransfer.States.v1;
using LocationService.Message.Definition.States.Requests.v1;
using LocationService.Message.Definition.States.Responses.v1;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace LocationService.Test.MockBuilder;

public static class StateMockBuilder
{
    private static readonly Fixture Fixture = new();

    private static IRepository GenerateMockRepository(State location = null, int rowCount = 100)
    {
        var mockState = location ?? GenerateMockState();
        var mockCounties = GenerateMockDomainStateList(rowCount);
        
        var repository = Substitute.For<IRepository>();
        
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

    private static ICache GenerateMockObjectCache()
    {
        var cache = Substitute.For<ICache>();
        return cache;
    }

    private static List<State> GenerateMockDomainStateList(int count)
    {
        return Fixture.Build<State>()
            .Without(s => s.Cities)
            .With(s => s.Country, () => new Country())
            .With(s => s.LastUpdateDate, () => DateTime.Now)
            .With(s => s.LastUpdateUserId, () => "Test")
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
            .Without(s => s.Cities)
            .Without(s => s.Country)
            .With(s => s.LastUpdateDate, () => DateTime.Now)
            .With(s => s.LastUpdateUserId, () => "Test")
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
                GenerateMockEventBus());
        }
        
        if (typeof(T) == typeof(SoftDeleteStateHandler))
        {
            return new SoftDeleteStateHandler(NullLogger<SoftDeleteStateHandler>.Instance,
                GenerateMockRepository(location),
                GenerateMockEventBus());
        }
        
        if (typeof(T) == typeof(DeleteStateHandler))
        {
            return new DeleteStateHandler(NullLogger<DeleteStateHandler>.Instance,
                GenerateMockRepository(location),
                GenerateMockEventBus());
        }
        
        if (typeof(T) == typeof(CreateStateHandler))
        {
            var repository = GenerateMockRepository(location);
            repository.GetStateAsync(Arg.Any<Expression<Func<State, bool>>>()).Returns((State)null);
            return new CreateStateHandler(NullLogger<CreateStateHandler>.Instance,
                repository,
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
