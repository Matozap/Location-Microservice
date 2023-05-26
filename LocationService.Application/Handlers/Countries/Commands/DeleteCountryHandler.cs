using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bustr.Bus;
using DistributedCache.Core;
using LocationService.Application.Interfaces;
using LocationService.Domain;
using LocationService.Message.Common;
using LocationService.Message.Countries;
using LocationService.Message.Countries.Events;
using LocationService.Message.Countries.Requests;
using Mapster;
using MediatrBuilder;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Handlers.Countries.Commands;

public class DeleteCountryHandler : BuilderRequestHandler<DeleteCountry, CountryData>
{
    private readonly ILogger<DeleteCountryHandler> _logger;
    private readonly IRepository _repository;
    private readonly ICache _cache;
    private readonly IEventBus _eventBus;

    public DeleteCountryHandler(ILogger<DeleteCountryHandler> logger, IRepository repository, ICache cache, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _cache = cache;
        _eventBus = eventBus;
    }
    
    protected override Task<CountryData> PreProcess(DeleteCountry request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<CountryData>(null);
    }

    protected override async Task<CountryData> Process(DeleteCountry request, CancellationToken cancellationToken = default)
    {
        var entity = await DeleteCountryAsync(request.Id);
        
        _logger.LogInformation("Country with id {CountryID} deleted successfully", entity?.Id);

        return entity.Adapt<Country, CountryData>();
    }

    protected override async Task PostProcess(DeleteCountry request, CountryData response, CancellationToken cancellationToken = default)
    {
        await ClearCache(response, cancellationToken);
        await _eventBus.PublishAsync(new CountryEvent { Details = response, Action = EventAction.Deleted });
    }

    private async Task<Country> DeleteCountryAsync(string id)
    {
        var entity = await _repository.GetAsSingleAsync<Country, string>(c => c.Id == id || c.Code == id, includeNavigationalProperties: true);

        if (entity == null) return null;

        if (entity.States?.Count > 0)
        {
            foreach (var state in entity.States.ToList())
            {
                if (state.Cities?.Count > 0)
                {
                    foreach (var city in state.Cities.ToList())
                    {
                        await _repository.DeleteAsync(city);
                    }
                }
                await _repository.DeleteAsync(state);
            }
        }

        await _repository.DeleteAsync(entity);

        return entity;
    }
    
    private async Task ClearCache(CountryData data, CancellationToken cancellationToken)
    {
        await _cache.ClearCacheWithPrefixAsync($"{nameof(Country)}:list", cancellationToken);
        await _cache.ClearCacheWithPrefixAsync($"{nameof(Country)}:id:{data?.Id}", cancellationToken);
    }
}
