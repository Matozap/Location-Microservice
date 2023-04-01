using System.Threading.Tasks;
using LocationService.API.Outputs;
using LocationService.API.Outputs.Base;
using LocationService.Message.DataTransfer.States.v1;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace LocationService.API.Inputs.Functions;

public class StateFunction : StateOutput
{
    public StateFunction(IMediator mediator) : base(mediator, OutputType.AzureFunction)
    {
    }
    
    [Function($"State-{nameof(GetAll)}")]
    public async Task<HttpResponseData> GetAll([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/states/{countryId}")] HttpRequestData req, string countryId)
        => await GetAllAsync<HttpResponseData>(countryId, req);
    
    [Function($"State-{nameof(Get)}")]
    public async Task<HttpResponseData> Get([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/state/{id}")] HttpRequestData req, string id)
        => await GetAsync<HttpResponseData>(id, req);
    
    [Function($"State-{nameof(Create)}")]
    public async Task<HttpResponseData> Create([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/state")] HttpRequestData req)
        => await CreateAsync<HttpResponseData>(await req.ReadFromJsonAsync<StateData>(), req);
    
    [Function($"State-{nameof(Update)}")]
    public async Task<HttpResponseData> Update([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "v1/state")] HttpRequestData req)
        => await UpdateAsync<HttpResponseData>(await req.ReadFromJsonAsync<StateData>(), req);
    
    [Function($"State-{nameof(Disable)}")]
    public async Task<HttpResponseData> Disable([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/state/disable/{id}" )] HttpRequestData req, string id)
        => await DisableAsync<HttpResponseData>(id, req);
    
    [Function($"State-{nameof(Delete)}")]
    public async Task<HttpResponseData> Delete([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/state/{id}")] HttpRequestData req, string id)
        => await DeleteAsync<HttpResponseData>(id, req);
}