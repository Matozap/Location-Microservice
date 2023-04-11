using System;
using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Application.Logic.Countries.v1.Requests;
using LocationService.Domain;
using LocationService.Message.Definition.Protos.Countries.v1;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Logic.Countries.v1.Queries;

public class GetCountryByIdHandler : IRequestHandler<GetCountryById, CountryData>
{
    private readonly ILogger<GetCountryByIdHandler> _logger;
    private readonly IRepository _repository;
    private readonly ICache _cache;

    public GetCountryByIdHandler(IRepository repository, ICache cache, ILogger<GetCountryByIdHandler> logger)
    {
        _repository = repository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<CountryData> Handle(GetCountryById request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request.Id);
        var cacheKey = GetCacheKey(request.Id);

        var cachedValue = await _cache.GetCacheValueAsync<CountryData>(cacheKey, cancellationToken);
        if (cachedValue != null)
        {
            _logger.LogInformation("Cache value found for {Key}", cacheKey);
            return cachedValue;
        }

        var dataValue = await GetCountryById(request.Id);

        if(dataValue != null)
        {
            _ = _cache.SetCacheValueAsync(cacheKey, dataValue, cancellationToken);
        }
            
        return dataValue;
    }

    private async Task<CountryData> GetCountryById(string id)
    {
        var entity = await _repository.GetAsSingleAsync<Country, string>(predicate: e => e.Id == id || e.Code == id,
        includeNavigationalProperties: true);
        var resultDto = entity.Adapt<Country, CountryData>();
        return resultDto;
    }

    public static string GetCacheKey(string id) => $"Country:id:{id}";
}
