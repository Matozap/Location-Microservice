using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Message.DataTransfer.Cities.v1;
using LocationService.Message.Definition.Cities.Requests.v1;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Logic.Cities.Queries.v1;

public class GetAllCitiesHandler : IRequestHandler<GetAllCities, object>
{
    private readonly ILogger<GetAllCitiesHandler> _logger;
    private readonly ICache _cache;
    private readonly ILocationRepository _repository;

    public GetAllCitiesHandler(ICache cache, ILogger<GetAllCitiesHandler> logger, ILocationRepository repository)
    {
        _logger = logger;
        _cache = cache;
        _repository = repository;
    }

    public async Task<object> Handle(GetAllCities request, CancellationToken cancellationToken)
    {
        var cacheKey = GetCacheKey(request.StateId);

        var cachedValue = await _cache.GetCacheValueAsync<List<CityData>>(cacheKey, cancellationToken);
        if (cachedValue != null)
        {
            _logger.LogInformation("Cache value found for {CacheKey}", cacheKey);
            return cachedValue;
        }

        var dataValue = await GetAllCities(request.StateId);

        _ = _cache.SetCacheValueAsync(cacheKey, dataValue, cancellationToken);

        return dataValue;
    }

    private async Task<List<CityData>> GetAllCities(int stateId)
    {
        var allLocations = await _repository.GetAllCitiesAsync(stateId);
        return allLocations.Adapt<List<CityData>>();
    }
    
    public static string GetCacheKey(int id) => $"Cities:{id}";
}
