using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using LocationService.API.Outputs;
using LocationService.API.Outputs.Base;
using LocationService.Message.DataTransfer.States.v1;
using MediatR;

namespace LocationService.API.Inputs.Grpc;

public class StateService : Message.DataTransfer.States.v1.StateService.StateServiceBase
{
    private readonly StateOutput _stateOutput;
    
    public StateService(IMediator mediator)
    {
        _stateOutput = new StateOutput(mediator, OutputType.Grpc);
    }

    public override async Task<StateCollection> GetAll(AllStatesRequest request, ServerCallContext context) 
        => new() { States = { await _stateOutput.GetAllAsync<List<StateData>>(request.CountryId) }};
    
    public override async Task<StateData> Get(StateIdRequest request, ServerCallContext context) 
        => await _stateOutput.GetAsync<StateData>(request.Id);

    public override async Task<StateData> Create(StateData request, ServerCallContext context) 
        => await _stateOutput.CreateAsync<StateData>(request);
    
    public override async Task<StateData> Update(StateData request, ServerCallContext context) 
        => await _stateOutput.UpdateAsync<StateData>(request);
    
    public override async Task<Empty> Disable(StateIdRequest request, ServerCallContext context)
    {
        await _stateOutput.DeleteAsync<StateData>(request.Id);
        return new Empty();
    }
}