using System;
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

public class CreateCountryHandler : BuilderRequestHandler<CreateCountry, CountryData>
{
    private readonly ILogger<CreateCountryHandler> _logger;
    private readonly IRepository _repository;
    private readonly ICache _cache;
    private readonly IEventBus _eventBus;

    public CreateCountryHandler(ILogger<CreateCountryHandler> logger, IRepository repository, ICache cache, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _cache = cache;
        _eventBus = eventBus;
    }

    protected override Task<CountryData> PreProcess(CreateCountry request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<CountryData>(null);
    }

    protected override async Task<CountryData> Process(CreateCountry request, CancellationToken cancellationToken = default)
    {
        var resultEntity = await CreateCountry(request.Details);
        if (resultEntity == null) return null;

        _logger.LogInformation("Country with id {CountryID} created successfully", resultEntity.Id);
        return resultEntity.Adapt<Country, CountryData>();
    }

    protected override async Task PostProcess(CreateCountry request, CountryData response, CancellationToken cancellationToken = default)
    {
        await ClearCache(response, cancellationToken);
        await _eventBus.PublishAsync(new CountryEvent { Details = response, Action = EventAction.Created });
    }

    private async Task<Country> CreateCountry(CountryData country)
    {
        if (await _repository.GetAsSingleAsync<Country,string>(e => e.Code == country.Code || e.Name == country.Name) != null)
        {
            return null;
        }
        
        var entity = country.Adapt<CountryData, Country>();
        entity.LastUpdateUserId ??= "system";
        entity.LastUpdateDate = DateTime.Now;
        
        return await _repository.AddAsync(entity);
    }

    private async Task ClearCache(CountryData data, CancellationToken cancellationToken)
    {
        await _cache.ClearCacheWithPrefixAsync($"{nameof(Country)}:list", cancellationToken);
        await _cache.ClearCacheWithPrefixAsync($"{nameof(Country)}:id:{data?.Id}", cancellationToken);
    }
}
