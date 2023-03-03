using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Message.DTO.States.v1;
using LocationService.Message.Messaging;
using LocationService.Message.Messaging.Event;
using LocationService.Message.Messaging.Event.v1;
using LocationService.Message.Messaging.Request.States.v1;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Commands.States.v1;

public class UpdateStateHandler : IRequestHandler<UpdateState, object>
{
    private readonly ILogger<UpdateStateHandler> _logger;
    private readonly ILocationRepository _repository;
    private readonly IMediator _mediator;
    private readonly IEventBus _eventBus;

    public UpdateStateHandler(ILogger<UpdateStateHandler> logger, ILocationRepository repository, IMediator mediator, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _mediator = mediator;
        _eventBus = eventBus;
    }

    public async Task<object> Handle(UpdateState request, CancellationToken cancellationToken)
    {
        await UpdateState(request.LocationDetails);
        _logger.LogInformation("State with id {StateID} updated successfully", request.LocationDetails.Id.ToString());
        _ = _eventBus.Publish(new StateEvent { LocationDetails = request.LocationDetails, Action = EventAction.StateUpdate});
            
        return request.LocationDetails;
    }

    private async Task UpdateState(StateData stateData)
    {
        var query = new GetStateById
        {
            Id = stateData.Id,
            Code = stateData.Code,
            Source = MessageSource.Command
        };
        var readResult = await _mediator.Send(query);
        var existingLocationDto = (StateData)readResult;
            
        if(existingLocationDto != null)
        {                
            await _repository.UpdateAsync(stateData.Adapt<StateData, Domain.State>());
        }
    }
}
