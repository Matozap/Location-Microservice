using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Domain;
using LocationService.Message.DataTransfer.Cities.v1;
using LocationService.Message.Definition.Cities.Requests.v1;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Logic.Cities.Queries.v1;

public class GetAllCitiesHandler : IRequestHandler<GetAllCities, List<CityData>>
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

    public async Task<List<CityData>> Handle(GetAllCities request, CancellationToken cancellationToken)
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

    private async Task<List<CityData>> GetAllCities(string stateId)
    {
        var parentByCode = await _repository.GetAsSingleAsync<State,string>(state => state.Id == stateId || state.Code == stateId) ?? new State();
        var entities = await _repository.GetAsListAsync<City,string>(
            predicate: city => (city.StateId == stateId || city.StateId == parentByCode.Id) && !city.Disabled,
            orderAscending: city => city.Name,
            includeNavigationalProperties: true);
        
        return entities.Adapt<List<CityData>>();
    }
    
    public static string GetCacheKey(string id) => $"Cities:{id}";
}
