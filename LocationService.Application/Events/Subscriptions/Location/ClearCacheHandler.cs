using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Handlers.Cities.v1.Queries;
using LocationService.Application.Handlers.Countries.v1.Queries;
using LocationService.Application.Handlers.States.v1.Queries;
using LocationService.Application.Interfaces;
using LocationService.Message.Events.Cache;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Events.Subscriptions.Location;

[ExcludeFromCodeCoverage]
public class ClearCacheHandler : IRequestHandler<ClearCache, bool>
{
    private readonly ILogger<ClearCacheHandler> _logger;
    private readonly ICache _cache;

    public ClearCacheHandler(ILogger<ClearCacheHandler> logger, ICache cache)
    {
        _logger = logger;
        _cache = cache;
    }

    public async Task<bool> Handle(ClearCache request, CancellationToken cancellationToken)
    {            
        if(request.ClearAll)
        {
            await _cache.ClearCacheAsync(cancellationToken);
        }
        else
        {
            
            if (!string.IsNullOrEmpty(request.CountryId))
            {
                const string message = "Clearing location by country cache";
                _logger.LogDebug(message);
                var cacheKey = GetCountryByIdHandler.GetCacheKey(request.CountryId);
                await _cache.RemoveValueAsync(cacheKey, cancellationToken);
                
                cacheKey = GetAllCountriesHandler.GetCacheKey();
                await _cache.RemoveValueAsync(cacheKey, cancellationToken);
            }
            
            if (!string.IsNullOrEmpty(request.StateCode) || !string.IsNullOrEmpty(request.StateId))
            {
                const string message = "Clearing location by state cache";
                _logger.LogDebug(message);
                var cacheKey = GetStateByIdHandler.GetCacheKey(request.StateId);
                await _cache.RemoveValueAsync(cacheKey, cancellationToken);
                
                cacheKey = GetAllStatesHandler.GetCacheKey(request.CountryId);
                await _cache.RemoveValueAsync(cacheKey, cancellationToken);
            }
            
            if (!string.IsNullOrEmpty(request.CityId))
            {
                const string message = "Clearing location by city cache";
                _logger.LogDebug(message);
                var cacheKey = GetCityByIdHandler.GetCacheKey(request.CityId);
                await _cache.RemoveValueAsync(cacheKey, cancellationToken);
                
                cacheKey = GetAllCitiesHandler.GetCacheKey(request.StateId);
                await _cache.RemoveValueAsync(cacheKey, cancellationToken);
            }
        }

        return true;
    }
}
