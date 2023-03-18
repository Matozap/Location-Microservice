using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Domain;
using LocationService.Message.DataTransfer.Cities.v1;
using LocationService.Message.Definition;
using LocationService.Message.Definition.Cities.Events.v1;
using LocationService.Message.Definition.Cities.Requests.v1;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Logic.Cities.Commands.v1;

public class DeleteCityHandler : IRequestHandler<DeleteCity, object>
{
    private readonly ILogger<DeleteCityHandler> _logger;
    private readonly ILocationRepository _repository;
    private readonly IEventBus _eventBus;

    public DeleteCityHandler(ILogger<DeleteCityHandler> logger, ILocationRepository repository, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _eventBus = eventBus;
    }

    public async Task<object> Handle(DeleteCity request, CancellationToken cancellationToken)
    {
        var entity = await DeleteCityAsync(request.Id);

        if (entity != null)
        {
            var publishData = entity.Adapt<City, CityData>();
            _ = _eventBus.Publish(new CityEvent { LocationDetails = publishData, Action = EventAction.CityDelete });
        }

        return request.Id;
    }

    private async Task<City> DeleteCityAsync(string cityId)
    {
        var entity = await _repository.GetCityAsync(c => c.Id == cityId);
        
        if(entity != null)
        {                
            await _repository.DeleteAsync(entity);
            _logger.LogInformation("City with id {CityId} was completely deleted", cityId);
        }
        
        return entity;
    }
}
