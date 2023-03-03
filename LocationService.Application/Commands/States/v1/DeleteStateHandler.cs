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

public class DeleteStateHandler : IRequestHandler<DeleteState, object>
{
    private readonly ILogger<DeleteStateHandler> _logger;
    private readonly ILocationRepository _repository;
    private readonly IMediator _mediator;
    private readonly IEventBus _eventBus;

    public DeleteStateHandler(ILogger<DeleteStateHandler> logger, ILocationRepository repository, IMediator mediator, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _mediator = mediator;
        _eventBus = eventBus;
    }

    public async Task<object> Handle(DeleteState request, CancellationToken cancellationToken)
    {
        await DeleteStateAsync(request.Id);

        _ = _eventBus.Publish(new StateEvent { LocationDetails = new StateData {Id = request.Id}, Action = EventAction.StateDelete});

        return request.Id.ToString();
    }

    private async Task DeleteStateAsync(int stateId)
    {
        var query = new GetStateById
        {
            Id = stateId,
            Source = MessageSource.Command
        };
        var readResult = await _mediator.Send(query);
        var existingLocationDto = (StateData)readResult;
            
        if(existingLocationDto != null)
        {                
            await _repository.DeleteAsync(existingLocationDto.Adapt<StateData, Domain.State>());
            _logger.LogInformation("State with id {StateId} was completely deleted", stateId.ToString());
        }
    }
}
