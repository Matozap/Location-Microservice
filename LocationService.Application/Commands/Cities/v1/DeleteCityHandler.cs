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

public class DeleteCityHandler : IRequestHandler<DeleteCity, object>
{
    private readonly ILogger<DeleteCityHandler> _logger;
    private readonly ILocationRepository _repository;
    private readonly IMediator _mediator;
    private readonly IEventBus _eventBus;

    public DeleteCityHandler(ILogger<DeleteCityHandler> logger, ILocationRepository repository, IMediator mediator, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _mediator = mediator;
        _eventBus = eventBus;
    }

    public async Task<object> Handle(DeleteCity request, CancellationToken cancellationToken)
    {
        await DeleteCityAsync(request.CityId);

        _ = _eventBus.Publish(new CityEvent { LocationDetails = new CityFlatData {Id = request.CityId}, Action = EventAction.CityDelete});

        return request.CityId.ToString();
    }

    private async Task DeleteCityAsync(int cityId)
    {
        var query = new GetCityById
        {
            CityId = cityId,
            Source = MessageSource.Command
        };
        var readResult = await _mediator.Send(query);
        var existingLocationDto = (CityData)readResult;
            
        if(existingLocationDto != null)
        {                
            await _repository.DeleteAsync(existingLocationDto.Adapt<CityData, Domain.City>());
            _logger.LogInformation("City with id {CityId} was completely deleted", cityId.ToString());
        }
    }
}
