using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Message.DataTransfer.Cities.v1;
using LocationService.Message.Definition;
using LocationService.Message.Definition.Cities.Requests.v1;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Logic.Cities.Queries.v1;

public class GetCityByIdHandler : IRequestHandler<GetCityById, object>
{
    private readonly ILogger<GetCityByIdHandler> _logger;
    private readonly ILocationRepository _repository;
    private readonly ICache _cache;

    public GetCityByIdHandler(ILocationRepository repository, ICache cache, ILogger<GetCityByIdHandler> logger)
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
