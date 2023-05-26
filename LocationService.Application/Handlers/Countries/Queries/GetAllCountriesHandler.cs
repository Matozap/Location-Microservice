using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DistributedCache.Core;
using LocationService.Application.Interfaces;
using LocationService.Domain;
using LocationService.Message.Countries;
using LocationService.Message.Countries.Requests;
using Mapster;
using MediatrBuilder;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Handlers.Countries.Queries;

public class GetAllCountriesHandler : BuilderRequestHandler<GetAllCountries, List<CountryData>>
{
    private readonly ILogger<GetAllCountriesHandler> _logger;
    private readonly ICache _cache;
    private readonly IRepository _repository;

    public GetAllCountriesHandler(ICache cache, ILogger<GetAllCountriesHandler> logger, IRepository repository)
    {
        _logger = logger;
        _cache = cache;
        _repository = repository;
    }

    protected override async Task<List<CountryData>> PreProcess(GetAllCountries request, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetCacheKey();

        var cachedValue = await _cache.GetCacheValueAsync<List<CountryData>>(cacheKey, cancellationToken);
        if (cachedValue == null) return null;
        
        _logger.LogInformation("Cache value found for {CacheKey}", cacheKey);
        return cachedValue;
    }
    
    protected override async Task<List<CountryData>> Process(GetAllCountries request, CancellationToken cancellationToken = default)
    {
        var entities = await _repository.GetAsListAsync<Country, string>(
            predicate: country => !country.Disabled, 
            orderAscending: country => country.Name, 
            selectExpression: country => new Country
            {
                Id = country.Id,
                Code = country.Code,
                Name = country.Name,
                Currency = country.Currency,
                CurrencyName = country.CurrencyName,
                Region = country.Region,
                SubRegion = country.SubRegion
            });
        return entities.Adapt<List<CountryData>>();
    }

    protected override Task PostProcess(GetAllCountries request, List<CountryData> response, CancellationToken cancellationToken = default)
    {
        if (response != null)
        {
            _ = _cache.SetCacheValueAsync(GetCacheKey(), response, cancellationToken);
        }

        return Task.CompletedTask;
    }
    
    private static string GetCacheKey() => $"{nameof(Country)}:list";
    
}
