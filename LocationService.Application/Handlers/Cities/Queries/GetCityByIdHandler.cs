using System.Threading;
using System.Threading.Tasks;
using DistributedCache.Core;
using LocationService.Application.Interfaces;
using LocationService.Domain;
using LocationService.Message.Cities;
using LocationService.Message.Cities.Requests;
using Mapster;
using MediatrBuilder;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Handlers.Cities.Queries;

public class GetCityByIdHandler : BuilderRequestHandler<GetCityById, CityData>
{
    private readonly ILogger<GetCityByIdHandler> _logger;
    private readonly IRepository _repository;
    private readonly ICache _cache;

    public GetCityByIdHandler(IRepository repository, ICache cache, ILogger<GetCityByIdHandler> logger)
    {
        _repository = repository;
        _cache = cache;
        _logger = logger;
    }
    
    protected override async Task<CityData> PreProcess(GetCityById request, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetCacheKey(request.Id);

        var cachedValue = await _cache.GetCacheValueAsync<CityData>(cacheKey, cancellationToken);
        if (cachedValue == null) return null;
        
        _logger.LogInformation("Cache value found for {Key}", cacheKey);
        return cachedValue;
    }

    protected override async Task<CityData> Process(GetCityById request, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetAsSingleAsync<City, string>(
            predicate: city => city.Id == request.Id && !city.Disabled,
            orderAscending: city => city.Name,
            includeNavigationalProperties: true);
        var resultDto = entity.Adapt<City, CityData>();
        return resultDto;
    }

    protected override Task PostProcess(GetCityById request, CityData response, CancellationToken cancellationToken = default)
    {
        if(response != null)
        {
            _ = _cache.SetCacheValueAsync(GetCacheKey(request.Id), response, cancellationToken);
        }
        return Task.CompletedTask;
    }

    private static string GetCacheKey(string id) => $"{nameof(City)}:id:{id}";
}
