using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Message.DataTransfer.Cities.v1;
using LocationService.Message.Definition;
using LocationService.Message.Definition.Cities.Events.v1;
using LocationService.Message.Definition.Cities.Requests.v1;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Logic.Cities.Commands.v1;

public class SoftDeleteCityHandler : IRequestHandler<SoftDeleteCity, object>
{
    private readonly ILogger<SoftDeleteCityHandler> _logger;
    private readonly ILocationRepository _repository;
    private readonly IEventBus _eventBus;

    public SoftDeleteCityHandler(ILogger<SoftDeleteCityHandler> logger, ILocationRepository repository, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _eventBus = eventBus;
    }

    public async Task<object> Handle(SoftDeleteCity request, CancellationToken cancellationToken)
    {
        await UpdateCity(request.Id);

        _ = _eventBus.Publish(new CityEvent { LocationDetails = new CityData { Id = request.Id }, Action = EventAction.CityDelete});

        return request.Id;
    }

    private async Task UpdateCity(string cityId)
    {
        var entity = await _repository.GetCityAsync(c => c.Id == cityId);
        if(entity != null)
        {
            entity.Disabled = true;
            await _repository.UpdateAsync(entity);
            _logger.LogInformation("City with id {CityId} was soft deleted", cityId);
        }
    }
}
