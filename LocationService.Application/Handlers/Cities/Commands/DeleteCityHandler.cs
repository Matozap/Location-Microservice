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

public class DeleteCityHandler : BuilderRequestHandler<DeleteCity, CityData>
{
    private readonly ILogger<DeleteCityHandler> _logger;
    private readonly IRepository _repository;
    private readonly ICache _cache;
    private readonly IEventBus _eventBus;

    public DeleteCityHandler(ILogger<DeleteCityHandler> logger, IRepository repository, ICache cache, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _cache = cache;
        _eventBus = eventBus;
    }
    
    protected override Task<CityData> PreProcess(DeleteCity request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<CityData>(null);
    }

    protected override async Task<CityData> Process(DeleteCity request, CancellationToken cancellationToken = default)
    {
        var entity = await DeleteCityAsync(request.Id);
        
        _logger.LogInformation("City with id {CityId} deleted successfully", entity?.Id);
        
        return entity.Adapt<City, CityData>();
    }

    protected override async Task PostProcess(DeleteCity request, CityData response, CancellationToken cancellationToken = default)
    {
        await ClearCache(response, cancellationToken);
        await _eventBus.PublishAsync(new CityEvent { Details = response, Action = EventAction.Deleted });
    }

    private async Task<City> DeleteCityAsync(string cityId)
    {
        var entity = await _repository.GetAsSingleAsync<City, string>(city => city.Id == cityId);
        if (entity == null) return null;
        
        await _repository.DeleteAsync(entity);
        return entity;
    }
    
    private async Task ClearCache(CityData data, CancellationToken cancellationToken)
    {
        await _cache.ClearCacheWithPrefixAsync($"{nameof(City)}:id:{data?.Id}", cancellationToken);
        await _cache.ClearCacheWithPrefixAsync($"{nameof(City)}:list", cancellationToken);
        await _cache.ClearCacheWithPrefixAsync($"{nameof(State)}:id:{data?.StateId}", cancellationToken);
    }
}
