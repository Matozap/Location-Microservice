using System.Threading;
using System.Threading.Tasks;
using Bustr.Bus;
using DistributedCache.Core;
using LocationService.Application.Interfaces;
using LocationService.Domain;
using LocationService.Message.Cities;
using LocationService.Message.Cities.Events;
using LocationService.Message.Cities.Requests;
using LocationService.Message.Common;
using Mapster;
using MediatrBuilder;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Handlers.Cities.Commands;

public class UpdateCityHandler : BuilderRequestHandler<UpdateCity, CityData>
{
    private readonly ILogger<UpdateCityHandler> _logger;
    private readonly IRepository _repository;
    private readonly ICache _cache;
    private readonly IEventBus _eventBus;

    public UpdateCityHandler(ILogger<UpdateCityHandler> logger, IRepository repository, ICache cache, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _cache = cache;
        _eventBus = eventBus;
    }
    
    protected override Task<CityData> PreProcess(UpdateCity request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<CityData>(null);
    }

    protected override async Task<CityData> Process(UpdateCity request, CancellationToken cancellationToken = default)
    {
        var result = await UpdateCity(request.Details);
        
        _logger.LogInformation("City with id {CityID} updated successfully", request.Details.Id);

        return result.Adapt<City, CityData>();
    }

    protected override async Task PostProcess(UpdateCity request, CityData response, CancellationToken cancellationToken = default)
    {
        await ClearCache(response, cancellationToken);
        await _eventBus.PublishAsync(new CityEvent { Details = response, Action = EventAction.Updated });
    }

    private async Task<City> UpdateCity(CityData cityData)
    {
        var entity = await _repository.GetAsSingleAsync<City, string>(city => city.Id == cityData.Id);
        if (entity == null) return null;
        
        cityData.StateId = entity.StateId;
        var changes = cityData.Adapt(entity);
        
        await _repository.UpdateAsync(changes);
        return changes;
    }
    
    private async Task ClearCache(CityData data, CancellationToken cancellationToken)
    {
        await _cache.ClearCacheWithPrefixAsync($"{nameof(City)}:id:{data?.Id}", cancellationToken);
        await _cache.ClearCacheWithPrefixAsync($"{nameof(City)}:list", cancellationToken);
        await _cache.ClearCacheWithPrefixAsync($"{nameof(State)}:id:{data?.StateId}", cancellationToken);
    }
}
