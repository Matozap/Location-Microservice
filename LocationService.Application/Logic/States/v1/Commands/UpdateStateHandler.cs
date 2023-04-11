using System.Threading;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using LocationService.Application.Logic.States.v1.Requests;
using LocationService.Domain;
using LocationService.Message.Definition.Protos.States.v1;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.Application.Logic.States.v1.Commands;

public class UpdateStateHandler : IRequestHandler<UpdateState, StateData>
{
    private readonly ILogger<UpdateStateHandler> _logger;
    private readonly IRepository _repository;

    public UpdateStateHandler(ILogger<UpdateStateHandler> logger, IRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<StateData> Handle(UpdateState request, CancellationToken cancellationToken)
    {
        var result = await UpdateState(request.Details);
        _logger.LogInformation("State with id {StateID} updated successfully", request.Details.Id);
            
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
