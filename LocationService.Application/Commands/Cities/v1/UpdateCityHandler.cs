using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Message.DTO.Cities.v1;
using LocationService.Message.Messaging;
using LocationService.Message.Messaging.Event;
using LocationService.Message.Messaging.Event.v1;
using LocationService.Message.Messaging.Request.Cities.v1;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Commands.Cities.v1;

public class UpdateCityHandler : IRequestHandler<UpdateCity, object>
{
    private readonly ILogger<UpdateCityHandler> _logger;
    private readonly ILocationRepository _repository;
    private readonly IMediator _mediator;
    private readonly IEventBus _eventBus;

    public UpdateCityHandler(ILogger<UpdateCityHandler> logger, ILocationRepository repository, IMediator mediator, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _mediator = mediator;
        _eventBus = eventBus;
    }

    public async Task<object> Handle(UpdateCity request, CancellationToken cancellationToken)
    {
        await UpdateCity(request.LocationDetails);
        _logger.LogInformation("City with id {CityID} updated successfully", request.LocationDetails.Id.ToString());
        _ = _eventBus.Publish(new CityEvent { LocationDetails = request.LocationDetails, Action = EventAction.CityUpdate});
            
        return request.LocationDetails;
    }

    private async Task UpdateCity(CityData cityData)
    {
        var query = new GetCityById
        {
            Id = cityData.Id,
            Source = MessageSource.Command
        };
        var readResult = await _mediator.Send(query);
        var existingresultDto = (CityData)readResult;
            
        if(existingresultDto != null)
        {                
            await _repository.UpdateAsync(cityData.Adapt<CityData, Domain.City>());
        }
    }
}
