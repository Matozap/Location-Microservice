using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bustr.Bus;
using DistributedCache.Core;
using LocationService.Application.Interfaces;
using LocationService.Domain;
using LocationService.Message.Contracts.States.v1;
using LocationService.Message.Contracts.States.v1.Requests;
using LocationService.Message.Events;
using LocationService.Message.Events.States.v1;
using Mapster;
using MediatrBuilder;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Handlers.States.v1.Commands;

public class DeleteStateHandler : BuilderRequestHandler<DeleteState, StateData>
{
    private readonly ILogger<DeleteStateHandler> _logger;
    private readonly IRepository _repository;
    private readonly ICache _cache;
    private readonly IEventBus _eventBus;

    public DeleteStateHandler(ILogger<DeleteStateHandler> logger, IRepository repository, ICache cache, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _cache = cache;
        _eventBus = eventBus;
    }
    
    protected override Task<StateData> PreProcess(DeleteState request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<StateData>(null);
    }

    protected override async Task<StateData> Process(DeleteState request, CancellationToken cancellationToken = default)
    {
        var entity = await DeleteStateAsync(request.Id);
        
        _logger.LogInformation("State with id {StateId} deleted successfully", entity?.Id);
        
        return entity.Adapt<State, StateData>();
    }

    protected override async Task PostProcess(DeleteState request, StateData response, CancellationToken cancellationToken = default)
    {
        await ClearCache(response, cancellationToken);
        await _eventBus.PublishAsync(new StateEvent { Details = response, Action = EventAction.Deleted });
    }

    private async Task<State> DeleteStateAsync(string stateId)
    {
        var entity = await _repository.GetAsSingleAsync<State, string>(c => c.Id == stateId || c.Code == stateId, includeNavigationalProperties: true);
        if (entity == null) return null;
        
        if (entity.Cities?.Count > 0)
        {
            foreach (var city in entity.Cities.ToList())
            {
                await _repository.DeleteAsync(city);
            }
        }
            
        await _repository.DeleteAsync(entity);
        return entity;
    }
    
    private async Task ClearCache(StateData data, CancellationToken cancellationToken)
    {
        await _cache.ClearCacheWithPrefixAsync($"{nameof(State)}:list", cancellationToken);
        await _cache.ClearCacheWithPrefixAsync($"{nameof(State)}:id:{data?.Id}", cancellationToken);
        await _cache.ClearCacheWithPrefixAsync($"{nameof(State)}:id:{data?.Code}", cancellationToken);
        await _cache.ClearCacheWithPrefixAsync($"{nameof(Country)}:id:{data?.CountryId}", cancellationToken);
    }
}
