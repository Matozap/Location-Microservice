using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Domain;
using LocationService.Message.DataTransfer.States.v1;
using LocationService.Message.Definition;
using LocationService.Message.Definition.States.Events.v1;
using LocationService.Message.Definition.States.Requests.v1;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Logic.States.Commands.v1;

public class DeleteStateHandler : IRequestHandler<DeleteState, object>
{
    private readonly ILogger<DeleteStateHandler> _logger;
    private readonly ILocationRepository _repository;
    private readonly IEventBus _eventBus;

    public DeleteStateHandler(ILogger<DeleteStateHandler> logger, ILocationRepository repository, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _eventBus = eventBus;
    }

    public async Task<object> Handle(DeleteState request, CancellationToken cancellationToken)
    {
        var entity = await DeleteStateAsync(request.Id);

        if (entity != null)
        {
            var publishData = entity.Adapt<State, StateData>();
            publishData.Cities = null;
            _ = _eventBus.Publish(new StateEvent { LocationDetails = publishData, Action = EventAction.StateDelete });
        }

        return request.Id;
    }

    private async Task<State> DeleteStateAsync(string stateId)
    {
        var entity = await _repository.GetStateAsync(c => c.Id == stateId || c.Code == stateId);
            
        if(entity != null)
        {                
            await _repository.DeleteAsync(entity);
            _logger.LogInformation("State with id {StateId} was completely deleted", stateId);
        }

        return entity;
    }
}
