using System;
using System.Threading;
using System.Threading.Tasks;
using Bustr.Bus;
using DistributedCache.Core;
using LocationService.Application.Cities.Events;
using LocationService.Application.Cities.Requests;
using LocationService.Application.Cities.Responses;
using LocationService.Application.Common;
using LocationService.Application.Common.Interfaces;
using LocationService.Domain;
using Mapster;
using MediatrBuilder;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Cities.Commands;

public class CreateCityHandler : BuilderRequestHandler<CreateCity, CityData>
{
    private readonly ILogger<CreateCityHandler> _logger;
    private readonly IRepository _repository;
    private readonly ICache _cache;
    private readonly IEventBus _eventBus;

    public CreateCityHandler(ILogger<CreateCityHandler> logger, IRepository repository, ICache cache, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _cache = cache;
        _eventBus = eventBus;
    }
    
    protected override Task<CityData> PreProcess(CreateCity request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<CityData>(null);
    }

    protected override async Task<CityData> Process(CreateCity request, CancellationToken cancellationToken = default)
    {
        var entity = await CreateCity(request.Details);
        if (entity == null) return null;

        _logger.LogInformation("City with id {CityID} created successfully", entity.Id);
        
        return entity.Adapt<City, CityData>();
    }

    protected override async Task PostProcess(CreateCity request, CityData response, CancellationToken cancellationToken = default)
    {
        await ClearCache(response, cancellationToken);
        await _eventBus.PublishAsync(new CityEvent { Details = response, Action = EventAction.Created });
    }

    private async Task<City> CreateCity(CityData city)
    {
        if (await _repository.GetAsSingleAsync<City, string>(e => e.Name == city.Name && e.StateId == city.StateId) != null)
        {
            return null;
        }
        
        var entity = city.Adapt<CityData, City>();
        entity.LastUpdateUserId ??= "system";
        entity.LastUpdateDate = DateTime.Now;
        
        return await _repository.AddAsync(entity);
    }
    
    private async Task ClearCache(CityData data, CancellationToken cancellationToken)
    {
        await _cache.ClearCacheWithPrefixAsync($"{nameof(City)}:id:{data?.Id}", cancellationToken);
        await _cache.ClearCacheWithPrefixAsync($"{nameof(City)}:list", cancellationToken);
        await _cache.ClearCacheWithPrefixAsync($"{nameof(State)}:id:{data?.StateId}", cancellationToken);
    }
}
