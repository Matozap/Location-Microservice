using System.Threading;
using System.Threading.Tasks;
using Bustr.Bus;
using DistributedCache.Core;
using LocationService.Application.Interfaces;
using LocationService.Domain;
using LocationService.Message.Common;
using LocationService.Message.States;
using LocationService.Message.States.Events;
using LocationService.Message.States.Requests;
using Mapster;
using MediatrBuilder;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Handlers.States.Commands;

public class UpdateStateHandler : BuilderRequestHandler<UpdateState, StateData>
{
    private readonly ILogger<UpdateStateHandler> _logger;
    private readonly IRepository _repository;
    private readonly ICache _cache;
    private readonly IEventBus _eventBus;

    public UpdateStateHandler(ILogger<UpdateStateHandler> logger, IRepository repository, ICache cache, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _cache = cache;
        _eventBus = eventBus;
    }
    
    protected override Task<StateData> PreProcess(UpdateState request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<StateData>(null);
    }

    protected override async Task<StateData> Process(UpdateState request, CancellationToken cancellationToken = default)
    {
        var result = await UpdateState(request.Details);
        
        _logger.LogInformation("State with id {StateID} updated successfully", request.Details.Id);
        
        return result.Adapt<State, StateData>();
    }

    protected override async Task PostProcess(UpdateState request, StateData response, CancellationToken cancellationToken = default)
    {
        await ClearCache(response, cancellationToken);
        await _eventBus.PublishAsync(new StateEvent { Details = response, Action = EventAction.Updated });
    }

    private async Task<State> UpdateState(StateData stateData)
    {
        var entity = await _repository.GetAsSingleAsync<State, string>(state => state.Id == stateData.Id || state.Code == stateData.Id);
        if (entity == null) return null;

        stateData.CountryId = entity.CountryId;
        var changes = stateData.Adapt(entity);
        
        await _repository.UpdateAsync(changes);
        return changes;
    }
    
    private async Task ClearCache(StateData data, CancellationToken cancellationToken)
    {
        await _cache.ClearCacheWithPrefixAsync($"{nameof(State)}:list", cancellationToken);
        await _cache.ClearCacheWithPrefixAsync($"{nameof(State)}:id:{data?.Id}", cancellationToken);
        await _cache.ClearCacheWithPrefixAsync($"{nameof(State)}:id:{data?.Code}", cancellationToken);
        await _cache.ClearCacheWithPrefixAsync($"{nameof(Country)}:id:{data?.CountryId}", cancellationToken);
    }
}
