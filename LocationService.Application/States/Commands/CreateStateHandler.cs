using System;
using System.Threading;
using System.Threading.Tasks;
using Bustr.Bus;
using DistributedCache.Core;
using LocationService.Application.Common;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.States.Events;
using LocationService.Application.States.Requests;
using LocationService.Application.States.Responses;
using LocationService.Domain;
using Mapster;
using MediatrBuilder;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.States.Commands;

public class CreateStateHandler : BuilderRequestHandler<CreateState, StateData>
{
    private readonly ILogger<CreateStateHandler> _logger;
    private readonly IRepository _repository;
    private readonly ICache _cache;
    private readonly IEventBus _eventBus;

    public CreateStateHandler(ILogger<CreateStateHandler> logger, IRepository repository, ICache cache, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _cache = cache;
        _eventBus = eventBus;
    }
    
    protected override Task<StateData> PreProcess(CreateState request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<StateData>(null);
    }

    protected override async Task<StateData> Process(CreateState request, CancellationToken cancellationToken = default)
    {
        var entity = await CreateState(request.Details);
        if (entity == null) return null;
        
        _logger.LogInformation("State with id {StateID} created successfully", entity.Id);
        
        var resultDto = entity.Adapt<State, StateData>();

        return resultDto;
    }

    protected override async Task PostProcess(CreateState request, StateData response, CancellationToken cancellationToken = default)
    {
        await ClearCache(response, cancellationToken);
        await _eventBus.PublishAsync(new StateEvent { Details = response, Action = EventAction.Created });
    }

    private async Task<State> CreateState(StateData state)
    {
        if (await _repository.GetAsSingleAsync<State, string>(e => e.Code == state.Code && e.CountryId == state.CountryId) != null)
        {
            return null;
        }
        
        var entity = state.Adapt<StateData, State>();
        entity.LastUpdateUserId ??= "system";
        entity.LastUpdateDate = DateTime.Now;
        
        return await _repository.AddAsync(entity);
    }
    
    private async Task ClearCache(StateData data, CancellationToken cancellationToken)
    {
        await _cache.ClearCacheWithPrefixAsync($"{nameof(State)}:list", cancellationToken);
        await _cache.ClearCacheWithPrefixAsync($"{nameof(State)}:id:{data?.Id}", cancellationToken);
        await _cache.ClearCacheWithPrefixAsync($"{nameof(State)}:id:{data?.Code}", cancellationToken);
        await _cache.ClearCacheWithPrefixAsync($"{nameof(Country)}:id:{data?.CountryId}", cancellationToken);
    }
}
