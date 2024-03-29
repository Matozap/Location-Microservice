using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoFixture;
using Bustr.Bus;
using DistributedCache.Core;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.States;
using LocationService.Application.States.Commands;
using LocationService.Application.States.Events;
using LocationService.Application.States.Queries;
using LocationService.Application.States.Requests;
using LocationService.Application.States.Responses;
using LocationService.Domain;
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
        var mockStatesList = GenerateMockDomainStateList(rowCount);
        
        var repository = Substitute.For<IRepository>();
        
        repository.AddAsync(Arg.Any<State>()).Returns(mockState);
        repository.UpdateAsync(mockState).Returns(mockState);
        repository.DeleteAsync(mockState).Returns(mockState);
        
        repository.GetAsSingleAsync(Arg.Any<Expression<Func<State, bool>>>(), Arg.Any<Expression<Func<State, string>>>(), 
            Arg.Any<Expression<Func<State, string>>>(), Arg.Any<Expression<Func<State, State>>>(), Arg.Any<bool>()).Returns(mockState);
        repository.GetAsListAsync(Arg.Any<Expression<Func<State, bool>>>(), Arg.Any<Expression<Func<State, string>>>(), 
            Arg.Any<Expression<Func<State, string>>>(), Arg.Any<Expression<Func<State, State>>>(), Arg.Any<bool>()).Returns(mockStatesList);
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
        eventBus.PublishAsync(Arg.Any<StateEvent>()).Returns(Task.CompletedTask);
        return eventBus;
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
                GenerateMockRepository(location), GenerateMockObjectCache(), GenerateMockEventBus());
        }
        
        if (typeof(T) == typeof(SoftDeleteStateHandler))
        {
            return new SoftDeleteStateHandler(NullLogger<SoftDeleteStateHandler>.Instance,
                GenerateMockRepository(location), GenerateMockObjectCache(), GenerateMockEventBus());
        }
        
        if (typeof(T) == typeof(DeleteStateHandler))
        {
            return new DeleteStateHandler(NullLogger<DeleteStateHandler>.Instance,
                GenerateMockRepository(location), GenerateMockObjectCache(), GenerateMockEventBus());
        }
        
        if (typeof(T) == typeof(CreateStateHandler))
        {
            var repository = GenerateMockRepository(location);
            repository.GetAsSingleAsync(Arg.Any<Expression<Func<State, bool>>>(), Arg.Any<Expression<Func<State, string>>>(), 
                Arg.Any<Expression<Func<State, string>>>(), Arg.Any<Expression<Func<State, State>>>(), Arg.Any<bool>()).Returns((State)null);
            return new CreateStateHandler(NullLogger<CreateStateHandler>.Instance,
                repository, GenerateMockObjectCache(), GenerateMockEventBus());
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
