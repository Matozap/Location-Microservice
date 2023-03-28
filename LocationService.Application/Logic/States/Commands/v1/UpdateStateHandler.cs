using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Domain;
using LocationService.Message.DataTransfer.States.v1;
using LocationService.Message.Definition.States.Requests.v1;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Logic.States.Commands.v1;

public class UpdateStateHandler : IRequestHandler<UpdateState, object>
{
    private readonly ILogger<UpdateStateHandler> _logger;
    private readonly IRepository _repository;

    public UpdateStateHandler(ILogger<UpdateStateHandler> logger, IRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<object> Handle(UpdateState request, CancellationToken cancellationToken)
    {
        var result = await UpdateState(request.LocationDetails);
        _logger.LogInformation("State with id {StateID} updated successfully", request.LocationDetails.Id);
            
        return result;
    }

    private async Task<StateData> UpdateState(StateData stateData)
    {
        var entity = await _repository.GetAsSingleAsync<State, string>(state => state.Id == stateData.Id || state.Code == stateData.Id);
        if (entity == null) return null;

        stateData.CountryId = entity.CountryId;
        var changes = stateData.Adapt(entity);
        
        await _repository.UpdateAsync(changes);
        return changes.Adapt<State, StateData>();
    }
}
