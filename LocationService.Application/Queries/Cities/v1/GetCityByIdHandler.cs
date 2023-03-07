using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Message.DTO.Cities.v1;
using LocationService.Message.Messaging;
using LocationService.Message.Messaging.Request.Cities.v1;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Queries.Cities.v1;

public class GetCityByIdHandler : IRequestHandler<GetCityById, object>
{
    private readonly ILogger<GetCityByIdHandler> _logger;
    private readonly ILocationRepository _repository;
    private readonly IObjectCache _cache;

    public GetCityByIdHandler(ILocationRepository repository, IObjectCache cache, ILogger<GetCityByIdHandler> logger)
    {
        _repository = repository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<object> Handle(GetCityById request, CancellationToken cancellationToken)
    {
        var cacheKey = GetCacheKey(request.Id.ToString());

        if (request.Source != MessageSource.Command)
        {
            var cachedValue = await _cache.GetCacheValueAsync<CityData>(cacheKey, cancellationToken);
            if (cachedValue != null)
            {
                _logger.LogInformation("Cache value found for {Key}", cacheKey);
                return cachedValue;
            }
        }

        var dataValue = await GetStateById(request.Id);

        if(dataValue != null)
        {
            _ = _cache.SetCacheValueAsync(cacheKey, dataValue, cancellationToken);
        }
            
        return dataValue;
    }

    private async Task<CityData> GetStateById(int id)
    {
        var entity = await _repository.GetCityAsync(e => e.Id == id);
        var resultDto = entity.Adapt<Domain.City, CityData>();
        return resultDto;
    }

    public static string GetCacheKey(string id) => $"City:id:{id}";
}
