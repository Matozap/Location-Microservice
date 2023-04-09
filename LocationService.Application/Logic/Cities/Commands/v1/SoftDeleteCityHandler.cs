using System;
using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Domain;
using LocationService.Message.Definition.Cities.Requests.v1;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Logic.Cities.Commands.v1;

public class SoftDeleteCityHandler : IRequestHandler<SoftDeleteCity, string>
{
    private readonly ILogger<SoftDeleteCityHandler> _logger;
    private readonly IRepository _repository;

    public SoftDeleteCityHandler(ILogger<SoftDeleteCityHandler> logger, IRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<string> Handle(SoftDeleteCity request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request.Id);
        
        var entity = await DisableCity(request.Id);
        _logger.LogInformation("City with id {CityId} disabled successfully", entity?.Id);
        return entity?.Id;
    }

    private async Task<City> DisableCity(string cityId)
    {
        var entity = await _repository.GetAsSingleAsync<City, string>(city => city.Id == cityId);
        if (entity == null) return null;
        
        entity.Disabled = true;
        await _repository.UpdateAsync(entity);

        return entity;
    }
}
