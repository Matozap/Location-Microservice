using System.Threading;
using System.Threading.Tasks;
using DistributedCache.Core;
using LocationService.Application.Interfaces;
using LocationService.Domain;
using LocationService.Message.Contracts.States.v1;
using LocationService.Message.Contracts.States.v1.Requests;
using Mapster;
using MediatrBuilder;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Handlers.States.v1.Queries;

public class GetStateByIdHandler : BuilderRequestHandler<GetStateById, StateData>
{
    private readonly ILogger<GetStateByIdHandler> _logger;
    private readonly IRepository _repository;
    private readonly ICache _cache;

    public GetStateByIdHandler(IRepository repository, ICache cache, ILogger<GetStateByIdHandler> logger)
    {
        _repository = repository;
        _cache = cache;
        _logger = logger;
    }
    
    protected override async Task<StateData> PreProcess(GetStateById request, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetCacheKey(request.Id);

        var cachedValue = await _cache.GetCacheValueAsync<StateData>(cacheKey, cancellationToken);
        if (cachedValue == null) return null;
        
        _logger.LogInformation("Cache value found for {Key}", cacheKey);
        return cachedValue;
    }

    protected override async Task<StateData> Process(GetStateById request, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetAsSingleAsync<State, string>(
            predicate: state => (state.Id == request.Id || state.Code == request.Id) && !state.Disabled,
            orderAscending: state => state.Name,
            includeNavigationalProperties: true);
        var resultDto = entity.Adapt<State, StateData>();
        return resultDto;
    }

    protected override Task PostProcess(GetStateById request, StateData response, CancellationToken cancellationToken = default)
    {
        if(response != null)
        {
            _ = _cache.SetCacheValueAsync(GetCacheKey(request.Id), response, cancellationToken);
        }
        return Task.CompletedTask;
    }

    private static string GetCacheKey(string id) => $"{nameof(State)}:id:{id}";
}
