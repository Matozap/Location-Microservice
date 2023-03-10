using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Message.DataTransfer.States.v1;
using LocationService.Message.Definition;
using LocationService.Message.Definition.States.Events.v1;
using LocationService.Message.Definition.States.Requests.v1;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Logic.States.Commands.v1;

public class SoftDeleteStateHandler : IRequestHandler<SoftDeleteState, object>
{
    private readonly ILogger<SoftDeleteStateHandler> _logger;
    private readonly ILocationRepository _repository;
    private readonly IMediator _mediator;
    private readonly IEventBus _eventBus;

    public SoftDeleteStateHandler(ILogger<SoftDeleteStateHandler> logger, ILocationRepository repository, IMediator mediator, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _mediator = mediator;
        _eventBus = eventBus;
    }

    public async Task<object> Handle(SoftDeleteState request, CancellationToken cancellationToken)
    {
        await UpdateState(request.Id);

        _ = _eventBus.Publish(new StateEvent { LocationDetails = new StateData { Id = request.Id }, Action = EventAction.StateDelete});

        return request.Id.ToString();
    }

    private async Task UpdateState(int stateId)
    {
        var query = new GetStateById
        {
            Id = stateId
        };
        var readResult = await _mediator.Send(query);
        var resultDto = (StateData)readResult;
            
        if(resultDto != null)
        {
            var result = resultDto.Adapt<StateData, Domain.State>();
            result.Disabled = true;
            await _repository.UpdateAsync(result);
            _logger.LogInformation("State with id {StateId} was soft deleted", stateId.ToString());
        }
    }
}
