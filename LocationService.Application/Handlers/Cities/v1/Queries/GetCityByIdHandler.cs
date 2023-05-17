using System.Threading;
using System.Threading.Tasks;
using DistributedCache.Core;
using LocationService.Application.Interfaces;
using LocationService.Domain;
using LocationService.Message.Contracts.Cities.v1;
using LocationService.Message.Contracts.Cities.v1.Requests;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Handlers.Cities.v1.Queries;

public class GetCityByIdHandler : IRequestHandler<GetCityById, CityData>
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

    public async Task<CityData> Handle(GetCityById request, CancellationToken cancellationToken)
    {
        var cacheKey = GetCacheKey(request.Id);

        var cachedValue = await _cache.GetCacheValueAsync<CityData>(cacheKey, cancellationToken);
        if (cachedValue != null)
        {
            _logger.LogInformation("Cache value found for {Key}", cacheKey);
            return cachedValue;
        }

        var dataValue = await GetStateById(request.Id);

        if(dataValue != null)
        {
            _ = _cache.SetCacheValueAsync(cacheKey, dataValue, cancellationToken);
        }
            
        return dataValue;
    }

    private async Task<CityData> GetStateById(string id)
    {
        var entity = await _repository.GetAsSingleAsync<City, string>(
            predicate: city => city.Id == id && !city.Disabled,
            orderAscending: city => city.Name,
            includeNavigationalProperties: true);
        var resultDto = entity.Adapt<City, CityData>();
        return resultDto;
    }

    public static string GetCacheKey(string id) => $"City:id:{id}";
}
