using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Application.Logic.Cities.v1.Queries;
using LocationService.Application.Logic.Countries.v1.Queries;
using LocationService.Application.Logic.States.v1.Queries;
using LocationService.Message.Definition.Events.Cache;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Events.Location;

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
            // No method to clear the complete cache as of now - https://github.com/dotnet/runtime/issues/36547
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
                var cacheKey = GetStateByIdHandler.GetCacheKey(request.StateId, request.StateCode);
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
