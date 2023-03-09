using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Message.DTO.Countries.v1;
using LocationService.Message.Messaging.Request.Countries.v1;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Queries.Countries.v1;

public class GetAllCountriesHandler : IRequestHandler<GetAllCountries, object>
{
    private readonly ILogger<GetAllCountriesHandler> _logger;
    private readonly ICache _cache;
    private readonly ILocationRepository _repository;

    public GetAllCountriesHandler(ICache cache, ILogger<GetAllCountriesHandler> logger, ILocationRepository repository)
    {
        _logger = logger;
        _cache = cache;
        _repository = repository;
    }

    public async Task<object> Handle(GetAllCountries request, CancellationToken cancellationToken)
    {
        var cacheKey = GetCacheKey();

        var cachedValue = await _cache.GetCacheValueAsync<List<CountryData>>(cacheKey, cancellationToken);
        if (cachedValue != null)
        {
            _logger.LogInformation("Cache value found for {CacheKey}", cacheKey);
            return cachedValue;
        }

        var dataValue = await GetAllCountries();

        _ = _cache.SetCacheValueAsync(cacheKey, dataValue, cancellationToken);

        return dataValue;
    }

    private async Task<List<CountryData>> GetAllCountries()
    {
        var allLocations = await _repository.GetAllCountriesAsync();
        return allLocations.Adapt<List<CountryData>>();
    }
    
    public static string GetCacheKey() => "Countries:All";
}
