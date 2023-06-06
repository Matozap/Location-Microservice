using System.Threading.Tasks;
using LocationService.API.Outputs;
using LocationService.API.Outputs.Base;
using LocationService.Application.States.Responses;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace LocationService.API.Endpoints.Functions;

public class StateFunction
{
    private readonly StateOutput _stateOutput;
    
    public StateFunction(IMediator mediator)
    {
        _stateOutput = new StateOutput(mediator, OutputType.AzureFunction);
    }
    
    [Function($"State-{nameof(GetAll)}")]
    public async Task<HttpResponseData> GetAll([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/states/{countryId}")] HttpRequestData req, string countryId)
        => await _stateOutput.GetAllAsync<HttpResponseData>(countryId, req);
    
    [Function($"State-{nameof(Get)}")]
    public async Task<HttpResponseData> Get([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/state/{id}")] HttpRequestData req, string id)
        => await _stateOutput.GetAsync<HttpResponseData>(id, req);
    
    [Function($"State-{nameof(Create)}")]
    public async Task<HttpResponseData> Create([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/state")] HttpRequestData req)
        => await _stateOutput.CreateAsync<HttpResponseData>(await req.ReadFromJsonAsync<StateData>(), req);
    
    [Function($"State-{nameof(Update)}")]
    public async Task<HttpResponseData> Update([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "v1/state")] HttpRequestData req)
        => await _stateOutput.UpdateAsync<HttpResponseData>(await req.ReadFromJsonAsync<StateData>(), req);
    
    [Function($"State-{nameof(Disable)}")]
    public async Task<HttpResponseData> Disable([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/state/disable/{id}" )] HttpRequestData req, string id)
        => await _stateOutput.DisableAsync<HttpResponseData>(id, req);
    
    [Function($"State-{nameof(Delete)}")]
    public async Task<HttpResponseData> Delete([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/state/{id}")] HttpRequestData req, string id)
        => await _stateOutput.DeleteAsync<HttpResponseData>(id, req);
}