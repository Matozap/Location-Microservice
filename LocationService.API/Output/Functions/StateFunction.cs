using System.Threading.Tasks;
using LocationService.API.Output.Base;
using LocationService.Message.DataTransfer.States.v1;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace LocationService.API.Output.Functions;

public class StateFunction : StateOutput
{
    public StateFunction(IMediator mediator) : base(mediator, OutputType.AzureFunction)
    {
    }
    
    [Function($"State-{nameof(GetAll)}")]
    public async Task<HttpResponseData> GetAll([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/states/{countryId}")] HttpRequestData req, string countryId)
        => await GetAllAsync(countryId, req) as HttpResponseData;
    
    [Function($"State-{nameof(Get)}")]
    public async Task<HttpResponseData> Get([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/state/{id}")] HttpRequestData req, string id)
        => await GetAsync(id, req) as HttpResponseData;
    
    [Function($"State-{nameof(Create)}")]
    public async Task<HttpResponseData> Create([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/state")] HttpRequestData req)
        => await CreateAsync(await req.ReadFromJsonAsync<StateData>(), req) as HttpResponseData;
    
    [Function($"State-{nameof(Update)}")]
    public async Task<HttpResponseData> Update([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "v1/state")] HttpRequestData req)
        => await UpdateAsync(await req.ReadFromJsonAsync<StateData>(), req) as HttpResponseData;
    
    [Function($"State-{nameof(Disable)}")]
    public async Task<HttpResponseData> Disable([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/state/disable/{id}" )] HttpRequestData req, string id)
        => await DisableAsync(id, req) as HttpResponseData;
    
    [Function($"State-{nameof(Delete)}")]
    public async Task<HttpResponseData> Delete([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/state/{id}")] HttpRequestData req, string id)
        => await DeleteAsync(id, req) as HttpResponseData;
}