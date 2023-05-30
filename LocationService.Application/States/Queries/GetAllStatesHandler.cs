using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DistributedCache.Core;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.States.Requests;
using LocationService.Application.States.Responses;
using LocationService.Domain;
using Mapster;
using MediatrBuilder;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.States.Queries;

public class GetAllStatesHandler : BuilderRequestHandler<GetAllStates, List<StateData>>
{
    private readonly ILogger<GetAllStatesHandler> _logger;
    private readonly ICache _cache;
    private readonly IRepository _repository;

    public GetAllStatesHandler(ICache cache, ILogger<GetAllStatesHandler> logger, IRepository repository)
    {
        _logger = logger;
        _cache = cache;
        _repository = repository;
    }

    protected override async Task<List<StateData>> PreProcess(GetAllStates request, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetCacheKey(request.CountryId);

        var cachedValue = await _cache.GetCacheValueAsync<List<StateData>>(cacheKey, cancellationToken);
        if (cachedValue == null) return null;
        
        _logger.LogInformation("Cache value found for {CacheKey}", cacheKey);
        return cachedValue;
    }

    protected override async Task<List<StateData>> Process(GetAllStates request, CancellationToken cancellationToken = default)
    {
        var parentByCode = await _repository.GetAsSingleAsync<Country, string>(predicate: e => e.Code == request.CountryId) ?? new Country();
        var entities = await _repository.GetAsListAsync<State, string>(
            predicate: state => (state.CountryId == request.CountryId || state.CountryId == parentByCode.Id) && !state.Disabled,
            orderAscending: state => state.Name,
            includeNavigationalProperties: true
        );
        return entities.Adapt<List<StateData>>();
    }

    protected override Task PostProcess(GetAllStates request, List<StateData> response, CancellationToken cancellationToken = default)
    {
        if (response != null)
        {
            _ = _cache.SetCacheValueAsync(GetCacheKey(request.CountryId), response, cancellationToken);
        }

        return Task.CompletedTask;
    }

    private static string GetCacheKey(string id) => $"{nameof(State)}:list:{id}";
}
