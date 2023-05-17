using System.Threading;
using System.Threading.Tasks;
using DistributedCache.Core;
using LocationService.Application.Interfaces;
using LocationService.Domain;
using LocationService.Message.Contracts.States.v1;
using LocationService.Message.Contracts.States.v1.Requests;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Handlers.States.v1.Queries;

public class GetStateByIdHandler : IRequestHandler<GetStateById, object>
{
    private readonly ILogger<GetStateByIdHandler> _logger;
    private readonly IRepository _repository;
    private readonly ICache _cache;

    public GetStateByIdHandler(IRepository repository, ICache cache, ILogger<GetStateByIdHandler> logger)
    {
        _repository = repository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<object> Handle(GetStateById request, CancellationToken cancellationToken)
    {
        var cacheKey = GetCacheKey(request.Id);

        var cachedValue = await _cache.GetCacheValueAsync<StateData>(cacheKey, cancellationToken);
        if (cachedValue != null)
        {
            _logger.LogInformation("Cache value found for {Key}", cacheKey);
            return cachedValue;
        }

        var dataValue = await GetStateById(request.Id);

        if(dataValue != null)
        {
            _ = _cache.SetCacheValueAsync(cacheKey, dataValue, cancellationToken);
        }
            
        return dataValue;
    }

    private async Task<StateData> GetStateById(string id)
    {
        var entity = await _repository.GetAsSingleAsync<State, string>(
            predicate: state => (state.Id == id || state.Code == id) && !state.Disabled,
            orderAscending: state => state.Name,
            includeNavigationalProperties: true);
        var resultDto = entity.Adapt<State, StateData>();
        return resultDto;
    }

    public static string GetCacheKey(string id) => $"State:id:{id}";
}
