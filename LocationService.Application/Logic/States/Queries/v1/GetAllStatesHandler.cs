using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Domain;
using LocationService.Message.DataTransfer.States.v1;
using LocationService.Message.Definition.States.Requests.v1;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Logic.States.Queries.v1;

public class GetAllStatesHandler : IRequestHandler<GetAllStates, object>
{
    private readonly ILogger<GetAllStatesHandler> _logger;
    private readonly ICache _cache;
    private readonly IRepository _repository;

    public GetAllStatesHandler(ICache cache, ILogger<GetAllStatesHandler> logger, IRepository repository)
    {
        _logger = logger;
        _cache = cache;
        _repository = repository;
    }

    public async Task<object> Handle(GetAllStates request, CancellationToken cancellationToken)
    {
        var cacheKey = GetCacheKey(request.CountryId);

        var cachedValue = await _cache.GetCacheValueAsync<List<StateData>>(cacheKey, cancellationToken);
        if (cachedValue != null)
        {
            _logger.LogInformation("Cache value found for {CacheKey}", cacheKey);
            return cachedValue;
        }

        var dataValue = await GetAllStates(request.CountryId);

        _ = _cache.SetCacheValueAsync(cacheKey, dataValue, cancellationToken);

        return dataValue;
    }

    private async Task<List<StateData>> GetAllStates(string countryId)
    {
        var parentByCode = await _repository.GetAsSingleAsync<Country, string>(predicate: e => e.Code == countryId) ?? new Country();
        var entities = await _repository.GetAsListAsync<State, string>(
            predicate: state => (state.CountryId == countryId || state.CountryId == parentByCode.Id) && !state.Disabled,
            orderAscending: state => state.Name,
            includeNavigationalProperties: true
        );
        return entities.Adapt<List<StateData>>();
    }
    
    public static string GetCacheKey(string id) => $"States:All:{id}";
}
