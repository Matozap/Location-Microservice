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

public class SoftDeleteStateHandler : BuilderRequestHandler<SoftDeleteState, StateData>
{
    private readonly ILogger<SoftDeleteStateHandler> _logger;
    private readonly IRepository _repository;
    private readonly ICache _cache;
    private readonly IEventBus _eventBus;

    public SoftDeleteStateHandler(ILogger<SoftDeleteStateHandler> logger, IRepository repository, ICache cache, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _cache = cache;
        _eventBus = eventBus;
    }
    
    protected override Task<StateData> PreProcess(SoftDeleteState request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<StateData>(null);
    }

    protected override async Task<StateData> Process(SoftDeleteState request, CancellationToken cancellationToken = default)
    {
        var entity = await DisableState(request.Id);
        
        _logger.LogInformation("State with id {StateId} disabled successfully", entity?.Id);
        
        return entity.Adapt<State, StateData>();
    }

    protected override async Task PostProcess(SoftDeleteState request, StateData response, CancellationToken cancellationToken = default)
    {
        await ClearCache(response, cancellationToken);
        await _eventBus.PublishAsync(new StateEvent { Details = response, Action = EventAction.Deleted });
    }

    private async Task<State> DisableState(string stateId)
    {
        var entity = await _repository.GetAsSingleAsync<State, string>(c => c.Id == stateId || c.Code == stateId);
        if (entity == null) return null;
        
        entity.Disabled = true;
        await _repository.UpdateAsync(entity);

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
