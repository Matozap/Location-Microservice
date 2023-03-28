using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Domain;
using LocationService.Message.DataTransfer.Countries.v1;
using LocationService.Message.Definition.Countries.Requests.v1;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Logic.Countries.Queries.v1;

public class GetAllCountriesHandler : IRequestHandler<GetAllCountries, object>
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
    
    public static string GetCacheKey() => "Countries:All";
}
