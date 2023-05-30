using System.Threading;
using System.Threading.Tasks;
using Bustr.Bus;
using DistributedCache.Core;
using LocationService.Application.Common;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Countries.Events;
using LocationService.Application.Countries.Requests;
using LocationService.Application.Countries.Responses;
using LocationService.Domain;
using Mapster;
using MediatrBuilder;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Countries.Commands;

public class SoftDeleteCountryHandler : BuilderRequestHandler<SoftDeleteCountry, CountryData>
{
    private readonly ILogger<SoftDeleteCountryHandler> _logger;
    private readonly IRepository _repository;
    private readonly ICache _cache;
    private readonly IEventBus _eventBus;

    public SoftDeleteCountryHandler(ILogger<SoftDeleteCountryHandler> logger, IRepository repository, ICache cache, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _cache = cache;
        _eventBus = eventBus;
    }
    
    protected override Task<CountryData> PreProcess(SoftDeleteCountry request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<CountryData>(null);
    }

    protected override async Task<CountryData> Process(SoftDeleteCountry request, CancellationToken cancellationToken = default)
    {
        var entity = await DisableCountry(request.Id);
        
        _logger.LogInformation("Country with id {CountryID} disabled successfully", entity?.Id);
        
        return entity.Adapt<Country, CountryData>();
    }

    protected override async Task PostProcess(SoftDeleteCountry request, CountryData response, CancellationToken cancellationToken = default)
    {
        await ClearCache(response, cancellationToken);
        await _eventBus.PublishAsync(new CountryEvent { Details = response, Action = EventAction.Deleted });
    }

    private async Task<Country> DisableCountry(string countryId)
    {
        var entity = await _repository.GetAsSingleAsync<Country, string>(c => c.Id == countryId || c.Code == countryId);
        if (entity == null) return null;
        
        entity.Disabled = true;
        await _repository.UpdateAsync(entity);

        return entity;
    }
    
    private async Task ClearCache(CountryData data, CancellationToken cancellationToken)
    {
        await _cache.ClearCacheWithPrefixAsync($"{nameof(Country)}:list", cancellationToken);
        await _cache.ClearCacheWithPrefixAsync($"{nameof(Country)}:id:{data?.Id}", cancellationToken);
    }
}
