using System.Threading.Tasks;
using LocationService.API.Outputs;
using LocationService.API.Outputs.Base;
using LocationService.Message.DataTransfer.Cities.v1;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace LocationService.API.Inputs.Functions;

public class CityFunction : CityOutput
{
    public CityFunction(IMediator mediator) : base(mediator, OutputType.AzureFunction)
    {
    }
    
    [Function($"City-{nameof(GetAll)}")]
    public async Task<HttpResponseData> GetAll([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/cities/{stateId}")] HttpRequestData req, string stateId)
        => await GetAllAsync<HttpResponseData>(stateId, req);
    
    [Function($"City-{nameof(Get)}")]
    public async Task<HttpResponseData> Get([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/city/{id}")] HttpRequestData req, string id)
        => await GetAsync<HttpResponseData>(id, req);
    
    [Function($"City-{nameof(Create)}")]
    public async Task<HttpResponseData> Create([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/city")] HttpRequestData req)
        => await CreateAsync<HttpResponseData>(await req.ReadFromJsonAsync<CityData>(), req);
    
    [Function($"City-{nameof(Update)}")]
    public async Task<HttpResponseData> Update([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "v1/city")] HttpRequestData req)
        => await UpdateAsync<HttpResponseData>(await req.ReadFromJsonAsync<CityData>(), req);
    
    [Function($"City-{nameof(Disable)}")]
    public async Task<HttpResponseData> Disable([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/city/disable/{id}" )] HttpRequestData req, string id)
        => await DisableAsync<HttpResponseData>(id, req);
    
    [Function($"City-{nameof(Delete)}")]
    public async Task<HttpResponseData> Delete([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/city/{id}")] HttpRequestData req, string id)
        => await DeleteAsync<HttpResponseData>(id, req);
}