using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Application.Logic.Countries.v1.Requests;
using LocationService.Domain;
using LocationService.Message.Contracts.Countries.v1;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Logic.Countries.v1.Queries;

public class GetAllCountriesHandler : IRequestHandler<GetAllCountries, List<CountryData>>
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

    public async Task<List<CountryData>> Handle(GetAllCountries request, CancellationToken cancellationToken)
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
