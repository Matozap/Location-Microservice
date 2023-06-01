using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DistributedCache.Core;
using LocationService.Application.Cities.Requests;
using LocationService.Application.Cities.Responses;
using LocationService.Application.Common.Interfaces;
using LocationService.Domain;
using Mapster;
using MediatrBuilder;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Cities.Queries;

public class GetAllCitiesHandler : BuilderRequestHandler<GetAllCities, List<CityData>>
{
    private readonly ILogger<GetAllCitiesHandler> _logger;
    private readonly ICache _cache;
    private readonly IRepository _repository;

    public GetAllCitiesHandler(ICache cache, ILogger<GetAllCitiesHandler> logger, IRepository repository)
    {
        _logger = logger;
        _cache = cache;
        _repository = repository;
    }
    
    protected override async Task<List<CityData>> PreProcess(GetAllCities request, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetCacheKey(request.StateId);

        var cachedValue = await _cache.GetCacheValueAsync<List<CityData>>(cacheKey, cancellationToken);
        if (cachedValue == null) return null;
        
        _logger.LogInformation("Cache value found for {CacheKey}", cacheKey);
        return cachedValue;
    }

    protected override async Task<List<CityData>> Process(GetAllCities request, CancellationToken cancellationToken = default)
    {
        var parentByCode = await _repository.GetAsSingleAsync<State,string>(state => state.Id == request.StateId || state.Code == request.StateId) ?? new State();
        var entities = await _repository.GetAsListAsync<City,string>(
            predicate: city => (city.StateId == request.StateId || city.StateId == parentByCode.Id) && !city.Disabled,
            orderAscending: city => city.Name,
            includeNavigationalProperties: true);
        
        return entities.Adapt<List<CityData>>();
    }

    protected override Task PostProcess(GetAllCities request, List<CityData> response, CancellationToken cancellationToken = default)
    {
        if (response != null)
        {
            _ = _cache.SetCacheValueAsync(GetCacheKey(request.StateId), response, cancellationToken);
        }

        return Task.CompletedTask;
    }

    private static string GetCacheKey(string id) => $"{nameof(City)}:list:{id}";
}
