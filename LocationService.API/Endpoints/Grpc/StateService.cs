using System.Collections.Generic;
using System.Threading.Tasks;
using LocationService.API.Outputs;
using LocationService.API.Outputs.Base;
using LocationService.Application.Common;
using LocationService.Application.States;
using LocationService.Application.States.Responses;
using MediatR;

namespace LocationService.API.Endpoints.Grpc;

public class StateService : IStateService
{
    private readonly StateOutput _stateOutput;
    
    public StateService(IMediator mediator)
    {
        _stateOutput = new StateOutput(mediator, OutputType.Grpc);
    }

    public async Task<List<StateData>> GetAll(StringWrapper stateId) => await _stateOutput.GetAllAsync<List<StateData>>(stateId.Value);
    
    public async Task<StateData> Get(StringWrapper id) => await _stateOutput.GetAsync<StateData>(id.Value);
    
    public async Task<StateData> Create(StateData data) => await _stateOutput.CreateAsync<StateData>(data);
    
    public async Task<StateData> Update(StateData data) => await _stateOutput.UpdateAsync<StateData>(data);
    
    public async Task Disable(StringWrapper id) => await _stateOutput.DisableAsync<StateData>(id.Value);

    public async Task Delete(StringWrapper id) => await _stateOutput.DeleteAsync<StateData>(id.Value);
}