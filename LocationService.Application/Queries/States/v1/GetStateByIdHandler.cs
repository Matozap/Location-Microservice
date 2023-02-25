using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Message.DTO.States.v1;
using LocationService.Message.Messaging;
using LocationService.Message.Messaging.Request.States.v1;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Queries.States.v1;

public class GetStateByIdHandler : IRequestHandler<GetStateById, object>
{
    private readonly ILogger<GetStateByIdHandler> _logger;
    private readonly ILocationRepository _repository;
    private readonly IObjectCache _cache;

    public GetStateByIdHandler(ILocationRepository repository, IObjectCache cache, ILogger<GetStateByIdHandler> logger)
    {
        _repository = repository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<object> Handle(GetStateById request, CancellationToken cancellationToken)
    {
        var cacheKey = GetCacheKey(request.Id, request.Code);

        if (request.Source != MessageSource.Command)
        {
            var cachedValue = await _cache.GetCacheValueAsync<StateData>(cacheKey, cancellationToken);
            if (cachedValue != null)
            {
                _logger.LogInformation("Cache value found for {Key}", cacheKey);
                return cachedValue;
            }
        }

        var dataValue = await GetStateById(request.Id, request.Code);

        if(dataValue != null)
        {
            _ = _cache.SetCacheValueAsync(cacheKey, dataValue, cancellationToken);
        }
            
        return dataValue;
    }

    private async Task<StateData> GetStateById(int id, string code)
    {
        var entity = await _repository.GetStateAsync(e => e.Id == id || e.Code == code);
        var locationDto = entity.Adapt<Domain.State, StateData>();
        return locationDto;
    }

    public static string GetCacheKey(int id, string code)
    {
        var key = id > 0 ? id.ToString() : code;
        return $"State:id:{key}";
    }
}
