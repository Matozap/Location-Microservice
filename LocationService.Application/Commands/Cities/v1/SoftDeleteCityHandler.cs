using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Message.DTO.Cities.v1;
using LocationService.Message.Messaging.Event;
using LocationService.Message.Messaging.Event.v1;
using LocationService.Message.Messaging.Request.Cities.v1;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Commands.Cities.v1;

public class SoftDeleteCityHandler : IRequestHandler<SoftDeleteCity, object>
{
    private readonly ILogger<SoftDeleteCityHandler> _logger;
    private readonly ILocationRepository _repository;
    private readonly IMediator _mediator;
    private readonly IEventBus _eventBus;

    public SoftDeleteCityHandler(ILogger<SoftDeleteCityHandler> logger, ILocationRepository repository, IMediator mediator, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _mediator = mediator;
        _eventBus = eventBus;
    }

    public async Task<object> Handle(SoftDeleteCity request, CancellationToken cancellationToken)
    {
        await UpdateCity(request.CityId);

        _ = _eventBus.Publish(new CityEvent { LocationDetails = new CityData { Id = request.CityId }, Action = EventAction.CityDelete});

        return request.CityId.ToString();
    }

    private async Task UpdateCity(int cityId)
    {
        var query = new GetCityById
        {
            CityId = cityId
        };
        var readResult = await _mediator.Send(query);
        var existingLocationDto = (CityData)readResult;
            
        if(existingLocationDto != null)
        {
            var result = existingLocationDto.Adapt<CityData, Domain.City>();
            result.Disabled = true;
            await _repository.UpdateAsync(result);
            _logger.LogInformation("City with id {CityId} was soft deleted", cityId.ToString());
        }
    }
}
