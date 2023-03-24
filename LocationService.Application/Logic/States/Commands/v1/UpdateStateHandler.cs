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

public class UpdateStateHandler : IRequestHandler<UpdateState, object>
{
    private readonly ILogger<UpdateStateHandler> _logger;
    private readonly IRepository _repository;
    private readonly IEventBus _eventBus;

    public UpdateStateHandler(ILogger<UpdateStateHandler> logger, IRepository repository, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _eventBus = eventBus;
    }

    public async Task<object> Handle(UpdateState request, CancellationToken cancellationToken)
    {
        var result = await UpdateState(request.LocationDetails);
        _logger.LogInformation("State with id {StateID} updated successfully", request.LocationDetails.Id);
        _ = _eventBus.Publish(new StateEvent { LocationDetails = request.LocationDetails, Action = EventAction.StateUpdate});
            
        return result;
    }

    private async Task<StateData> UpdateState(StateData stateData)
    {
        var entity = await _repository.GetStateAsync(c => c.Id == stateData.Id);
        if (entity == null) return null;

        stateData.CountryId = entity.CountryId;
        var changes = stateData.Adapt(entity);
        
        await _repository.UpdateAsync(changes);
        return changes.Adapt<Domain.State, StateData>();
    }
}
