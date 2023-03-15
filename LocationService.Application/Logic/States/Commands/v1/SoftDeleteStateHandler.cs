using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Message.DataTransfer.States.v1;
using LocationService.Message.Definition;
using LocationService.Message.Definition.States.Events.v1;
using LocationService.Message.Definition.States.Requests.v1;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Logic.States.Commands.v1;

public class SoftDeleteStateHandler : IRequestHandler<SoftDeleteState, object>
{
    private readonly ILogger<SoftDeleteStateHandler> _logger;
    private readonly ILocationRepository _repository;
    private readonly IEventBus _eventBus;

    public SoftDeleteStateHandler(ILogger<SoftDeleteStateHandler> logger, ILocationRepository repository, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _eventBus = eventBus;
    }

    public async Task<object> Handle(SoftDeleteState request, CancellationToken cancellationToken)
    {
        await UpdateState(request.Id);

        _ = _eventBus.Publish(new StateEvent { LocationDetails = new StateData { Id = request.Id }, Action = EventAction.StateDelete});

        return request.Id;
    }

    private async Task UpdateState(string stateId)
    {
        var entity = await _repository.GetStateAsync(c => c.Id == stateId || c.Code == stateId);
            
        if(entity != null)
        {
            entity.Disabled = true;
            await _repository.UpdateAsync(entity);
            _logger.LogInformation("State with id {StateId} was soft deleted", stateId);
        }
    }
}
