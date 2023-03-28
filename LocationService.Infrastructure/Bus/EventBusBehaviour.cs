using System;
using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Domain;
using LocationService.Infrastructure.Extensions;
using LocationService.Message.DataTransfer.Cities.v1;
using LocationService.Message.DataTransfer.Countries.v1;
using LocationService.Message.DataTransfer.States.v1;
using LocationService.Message.Definition;
using LocationService.Message.Definition.Cities.Events.v1;
using LocationService.Message.Definition.Countries.Events.v1;
using LocationService.Message.Definition.States.Events.v1;
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
                    await _eventBus.Publish(new CountryEvent { LocationDetails = countryData, Action = outboxMessage.Action});
                    break;
                    
                case EventAction.StateCreate:
                case EventAction.StateUpdate:
                case EventAction.StateDelete:
                    var state = outboxMessage.JsonObject.Deserialize<State>();
                    var stateData = state.Adapt<State, StateData>();
                    await _eventBus.Publish(new StateEvent { LocationDetails = stateData, Action = outboxMessage.Action});
                    break;
                    
                case EventAction.CityCreate:
                case EventAction.CityUpdate:
                case EventAction.CityDelete:
                    var city = outboxMessage.JsonObject.Deserialize<City>();
                    var cityData = city.Adapt<City, CityData>();
                    await _eventBus.Publish(new CityEvent { LocationDetails = cityData, Action = outboxMessage.Action});
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