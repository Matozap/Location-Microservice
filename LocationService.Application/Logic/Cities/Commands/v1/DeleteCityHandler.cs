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
        await DeleteCityAsync(request.Id);

        _ = _eventBus.Publish(new CityEvent { LocationDetails = new CityData {Id = request.Id}, Action = EventAction.CityDelete});

        return request.Id.ToString();
    }

    private async Task DeleteCityAsync(int cityId)
    {
        var query = new GetCityById
        {
            Id = cityId,
            Source = MessageSource.Command
        };
        var readResult = await _mediator.Send(query);
        var existingresultDto = (CityData)readResult;
            
        if(existingresultDto != null)
        {                
            await _repository.DeleteAsync(existingresultDto.Adapt<CityData, Domain.City>());
            _logger.LogInformation("City with id {CityId} was completely deleted", cityId.ToString());
        }
    }
}
