using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Message.DataTransfer.States.v1;
using LocationService.Message.Definition.States.Requests.v1;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Logic.States.Queries.v1;

public class GetAllStatesHandler : IRequestHandler<GetAllStates, object>
{
    private readonly ILogger<GetAllStatesHandler> _logger;
    private readonly ICache _cache;
    private readonly ILocationRepository _repository;

    public GetAllStatesHandler(ICache cache, ILogger<GetAllStatesHandler> logger, ILocationRepository repository)
    {
        _logger = logger;
        _cache = cache;
        _repository = repository;
    }

    public async Task<object> Handle(GetAllStates request, CancellationToken cancellationToken)
    {
        var cacheKey = GetCacheKey(request.CountryId);

        var cachedValue = await _cache.GetCacheValueAsync<List<StateData>>(cacheKey, cancellationToken);
        if (cachedValue != null)
        {
            _logger.LogInformation("Cache value found for {CacheKey}", cacheKey);
            return cachedValue;
        }

        var dataValue = await GetAllStates(request.CountryId);

        _ = _cache.SetCacheValueAsync(cacheKey, dataValue, cancellationToken);

        return dataValue;
    }

    private async Task<List<StateData>> GetAllStates(string countryId)
    {
        var allLocations = await _repository.GetAllStatesAsync(countryId);
        return allLocations.Adapt<List<StateData>>();
    }
    
    public static string GetCacheKey(string id) => $"States:All:{id}";
}
