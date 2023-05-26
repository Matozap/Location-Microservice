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

public class GetCountryByIdHandler : BuilderRequestHandler<GetCountryById, CountryData>
{
    private readonly ILogger<GetCountryByIdHandler> _logger;
    private readonly IRepository _repository;
    private readonly ICache _cache;

    public GetCountryByIdHandler(IRepository repository, ICache cache, ILogger<GetCountryByIdHandler> logger)
    {
        _repository = repository;
        _cache = cache;
        _logger = logger;
    }

    protected override async Task<CountryData> PreProcess(GetCountryById request, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetCacheKey(request.Id);

        var cachedValue = await _cache.GetCacheValueAsync<CountryData>(cacheKey, cancellationToken);
        if (cachedValue == null) return null;
        
        _logger.LogInformation("Cache value found for {Key}", cacheKey);
        return cachedValue;
    }

    protected override async Task<CountryData> Process(GetCountryById request, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetAsSingleAsync<Country, string>(
            predicate: e => e.Id == request.Id || e.Code == request.Id,
            includeNavigationalProperties: true);
        var resultDto = entity.Adapt<Country, CountryData>();
        return resultDto;
    }

    protected override Task PostProcess(GetCountryById request, CountryData response, CancellationToken cancellationToken = default)
    {
        if(response != null)
        {
            _ = _cache.SetCacheValueAsync(GetCacheKey(request.Id), response, cancellationToken);
        }
        return Task.CompletedTask;
    }
    
    private static string GetCacheKey(string id) => $"{nameof(Country)}:id:{id}";
}
