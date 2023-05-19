using System.Threading;
using System.Threading.Tasks;
using Bustr.Bus;
using DistributedCache.Core;
using LocationService.Application.Interfaces;
using LocationService.Domain;
using LocationService.Message.Contracts.Countries.v1;
using LocationService.Message.Contracts.Countries.v1.Requests;
using LocationService.Message.Events;
using LocationService.Message.Events.Countries.v1;
using Mapster;
using MediatrBuilder;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Handlers.Countries.v1.Commands;

public class UpdateCountryHandler : BuilderRequestHandler<UpdateCountry, CountryData>
{
    private readonly ILogger<UpdateCountryHandler> _logger;
    private readonly IRepository _repository;
    private readonly ICache _cache;
    private readonly IEventBus _eventBus;

    public UpdateCountryHandler(ILogger<UpdateCountryHandler> logger, IRepository repository, ICache cache, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _cache = cache;
        _eventBus = eventBus;
    }
    
    protected override Task<CountryData> PreProcess(UpdateCountry request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<CountryData>(null);
    }

    protected override async Task<CountryData> Process(UpdateCountry request, CancellationToken cancellationToken = default)
    {
        var result = await UpdateCountry(request.Details);
        
        _logger.LogInformation("Country with id {CountryID} updated successfully", request.Details.Id);
            
        return result.Adapt<Country, CountryData>();
    }

    protected override async Task PostProcess(UpdateCountry request, CountryData response, CancellationToken cancellationToken = default)
    {
        await ClearCache(response, cancellationToken);
        await _eventBus.PublishAsync(new CountryEvent { Details = response, Action = EventAction.Updated });
    }

    private async Task<Country> UpdateCountry(CountryData countryData)
    {
        var entity = await _repository.GetAsSingleAsync<Country, string>(e => e.Id == countryData.Id || e.Code == countryData.Code);
        if (entity == null) return null;
        
        var changes = countryData.Adapt(entity);
        await _repository.UpdateAsync(changes);
        return changes;
    }
    
    private async Task ClearCache(CountryData data, CancellationToken cancellationToken)
    {
        await _cache.ClearCacheWithPrefixAsync($"{nameof(Country)}:list", cancellationToken);
        await _cache.ClearCacheWithPrefixAsync($"{nameof(Country)}:id:{data?.Id}", cancellationToken);
    }
}
