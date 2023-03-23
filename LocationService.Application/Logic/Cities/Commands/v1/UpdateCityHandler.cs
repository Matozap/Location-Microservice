using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Message.DataTransfer.Cities.v1;
using LocationService.Message.Definition;
using LocationService.Message.Definition.Cities.Events.v1;
using LocationService.Message.Definition.Cities.Requests.v1;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Logic.Cities.Commands.v1;

public class UpdateCityHandler : IRequestHandler<UpdateCity, object>
{
    private readonly ILogger<UpdateCityHandler> _logger;
    private readonly IRepository _repository;
    private readonly IEventBus _eventBus;

    public UpdateCityHandler(ILogger<UpdateCityHandler> logger, IRepository repository, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _eventBus = eventBus;
    }

    public async Task<object> Handle(UpdateCity request, CancellationToken cancellationToken)
    {
        var result = await UpdateCity(request.LocationDetails);
        _logger.LogInformation("City with id {CityID} updated successfully", request.LocationDetails.Id);
        _ = _eventBus.Publish(new CityEvent { LocationDetails = request.LocationDetails, Action = EventAction.CityUpdate});
            
        return result;
    }

    private async Task<CityData> UpdateCity(CityData cityData)
    {
        var entity = await _repository.GetCityAsync(c => c.Id == cityData.Id);
        if (entity == null) return null;
        
        var changes = cityData.Adapt(entity);
        await _repository.UpdateAsync(changes);
        return changes.Adapt<Domain.City, CityData>();
    }
}
