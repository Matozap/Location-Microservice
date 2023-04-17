using System;
using System.Text.Json;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Domain;
using LocationService.Message.Contracts.Cities.v1;
using LocationService.Message.Contracts.Countries.v1;
using LocationService.Message.Contracts.States.v1;
using LocationService.Message.Events;
using LocationService.Message.Events.Cities.v1;
using LocationService.Message.Events.Countries.v1;
using LocationService.Message.Events.States.v1;
using Mapster;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Events.Publishers;

public class OutboxPublisher : IOutboxPublisher
{
    private readonly IEventBus _eventBus;
    private readonly IRepository _repository;
    private readonly ILogger<OutboxPublisher> _logger;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

    public OutboxPublisher(IEventBus eventBus, IRepository repository, ILogger<OutboxPublisher> logger)
    {
        _eventBus = eventBus;
        _repository = repository;
        _logger = logger;
    }

    public async Task PublishOutboxAsync()
    {
        var outboxMessages = await _repository.GetAsListAsync<Outbox, DateTime>(outbox => outbox.Id != "", outbox => outbox.LastUpdateDate);
        if (!(outboxMessages?.Count > 0)) return;
        
        _logger.LogInformation("Publishing events from outbox - Count: {MessageCount}", outboxMessages.Count.ToString());
        foreach (var outboxMessage in outboxMessages)
        {
            switch (outboxMessage.ObjectType)
            {
                case nameof(Country):
                    await PublishCountryEvent(outboxMessage);
                    break;
                    
                case nameof(State):
                    await PublishStateEvent(outboxMessage);
                    break;
                
                case nameof(City):
                    await PublishCityEvent(outboxMessage);
                    break;
                default:
                    _logger.LogWarning("Unknown entity found in EventBusOutbox - Entity Name: {ObjectType}", outboxMessage.ObjectType);
                    break;
            }

            await _repository.DeleteAsync(outboxMessage, skipOutbox: true);
        }
    } 
    
    private async Task PublishCountryEvent(Outbox outboxMessage)
    {
        var country =  JsonSerializer.Deserialize<Country>(outboxMessage.JsonObject, _jsonSerializerOptions);
        var countryData = country.Adapt<Country, CountryData>();
        
        switch (outboxMessage.Operation)
        {
            case Operation.Create:
                await _eventBus.Publish(new CountryEvent { Details = countryData, Action = EventAction.Created });
                break;
            case Operation.Update:
                await _eventBus.Publish(new CountryEvent { Details = countryData, Action = EventAction.Updated });
                break;
            case Operation.Delete:
                await _eventBus.Publish(new CountryEvent { Details = countryData, Action = EventAction.Deleted });
                break;
            case Operation.None:
            default:
                _logger.LogWarning("Unknown operation found in EventBusOutbox - Entity Name: {ObjectType}, Operation: {Operation}", outboxMessage.ObjectType, Enum.GetName(outboxMessage.Operation));
                break;
        }
    }
    
    private async Task PublishStateEvent(Outbox outboxMessage)
    {
        var state = JsonSerializer.Deserialize<State>(outboxMessage.JsonObject, _jsonSerializerOptions);
        var stateData = state.Adapt<State, StateData>();
        
        switch (outboxMessage.Operation)
        {
            case Operation.Create:
                await _eventBus.Publish(new StateEvent { Details = stateData, Action = EventAction.Created});
                break;
            case Operation.Update:
                await _eventBus.Publish(new StateEvent { Details = stateData, Action = EventAction.Updated});
                break;
            case Operation.Delete:
                await _eventBus.Publish(new StateEvent { Details = stateData, Action = EventAction.Deleted});
                break;
            case Operation.None:
            default:
                _logger.LogWarning("Unknown operation found in EventBusOutbox - Entity Name: {ObjectType}, Operation: {Operation}", outboxMessage.ObjectType, Enum.GetName(outboxMessage.Operation));
                break;
        }
    }
    
    private async Task PublishCityEvent(Outbox outboxMessage)
    {
        var city = JsonSerializer.Deserialize<City>(outboxMessage.JsonObject, _jsonSerializerOptions);
        var cityData = city.Adapt<City, CityData>();
        
        switch (outboxMessage.Operation)
        {
            case Operation.Create:
                await _eventBus.Publish(new CityEvent { Details = cityData, Action = EventAction.Created});
                break;
            case Operation.Update:
                await _eventBus.Publish(new CityEvent { Details = cityData, Action = EventAction.Updated});
                break;
            case Operation.Delete:
                await _eventBus.Publish(new CityEvent { Details = cityData, Action = EventAction.Deleted});
                break;
            case Operation.None:
            default:
                _logger.LogWarning("Unknown operation found in EventBusOutbox - Entity Name: {ObjectType}, Operation: {Operation}", outboxMessage.ObjectType, Enum.GetName(outboxMessage.Operation));
                break;
        }
    }
}