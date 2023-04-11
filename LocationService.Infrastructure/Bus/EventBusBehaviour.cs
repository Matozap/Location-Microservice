using System;
using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Domain;
using LocationService.Infrastructure.Extensions;
using LocationService.Message.Definition.Protos.Cities.v1;
using LocationService.Message.Definition.Protos.Countries.v1;
using LocationService.Message.Definition.Protos.States.v1;
using LocationService.Message.Definition.Events;
using LocationService.Message.Definition.Events.Cities.v1;
using LocationService.Message.Definition.Events.Countries.v1;
using LocationService.Message.Definition.Events.States.v1;
using Mapster;
using MediatR;

namespace LocationService.Infrastructure.Bus;

public class EventBusBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IEventBus _eventBus;
    private readonly IRepository _repository;
    
    public EventBusBehaviour(IEventBus eventBus, IRepository repository)
    {
        _eventBus = eventBus;
        _repository = repository;
    }
    
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var response = await next();

        var outboxMessages = await _repository.GetAsListAsync<EventBusOutbox, DateTime>(outbox => outbox.Id != "", outbox => outbox.LastUpdateDate);
        if (!(outboxMessages?.Count > 0)) return response;
        
        foreach (var outboxMessage in outboxMessages)
        {
            switch (outboxMessage.Action)
            {
                case EventAction.CountryCreate:
                case EventAction.CountryUpdate:
                case EventAction.CountryDelete:
                    var country = outboxMessage.JsonObject.Deserialize<Country>();
                    var countryData = country.Adapt<Country, CountryData>();
                    await _eventBus.Publish(new CountryEvent { Details = countryData, Action = outboxMessage.Action});
                    break;
                    
                case EventAction.StateCreate:
                case EventAction.StateUpdate:
                case EventAction.StateDelete:
                    var state = outboxMessage.JsonObject.Deserialize<State>();
                    var stateData = state.Adapt<State, StateData>();
                    await _eventBus.Publish(new StateEvent { Details = stateData, Action = outboxMessage.Action});
                    break;
                    
                case EventAction.CityCreate:
                case EventAction.CityUpdate:
                case EventAction.CityDelete:
                    var city = outboxMessage.JsonObject.Deserialize<City>();
                    var cityData = city.Adapt<City, CityData>();
                    await _eventBus.Publish(new CityEvent { Details = cityData, Action = outboxMessage.Action});
                    break;
                case EventAction.None:
                default:
                    throw new ArgumentOutOfRangeException();
            }

            await _repository.DeleteAsync(outboxMessage);
        }

        return response;
    }
}