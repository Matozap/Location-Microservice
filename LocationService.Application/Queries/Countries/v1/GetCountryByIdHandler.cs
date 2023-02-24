using System;
using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Message.DTO.Countries.v1;
using LocationService.Message.Messaging;
using LocationService.Message.Messaging.Request.Countries.v1;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Queries.Countries.v1;

public class GetCountryByIdHandler : IRequestHandler<GetCountryById, object>
{
    private readonly ILogger<GetCountryByIdHandler> _logger;
    private readonly ILocationRepository _repository;
    private readonly IObjectCache _cache;

    public GetCountryByIdHandler(ILocationRepository repository, IObjectCache cache, ILogger<GetCountryByIdHandler> logger)
    {
        _repository = repository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<object> Handle(GetCountryById request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request.CountryId);
        var cacheKey = GetCacheKey(request.CountryId);

        if (request.Source != MessageSource.Command)
        {
            var cachedValue = await _cache.GetCacheValueAsync<CountryData>(cacheKey, cancellationToken);
            if (cachedValue != null)
            {
                _logger.LogInformation("Cache value found for {Key}", cacheKey);
                return cachedValue;
            }
        }

        var dataValue = await GetCountryById(request.CountryId);

        if(dataValue != null)
        {
            _ = _cache.SetCacheValueAsync(cacheKey, dataValue, cancellationToken);
        }
            
        return dataValue;
    }

    private async Task<CountryData> GetCountryById(string id)
    {
        var entity = await _repository.GetCountryAsync(e => e.Id == id);
        var locationDto = entity.Adapt<Domain.Country, CountryData>();
        return locationDto;
    }

    public static string GetCacheKey(string id) => $"Country:id:{id}";
}
