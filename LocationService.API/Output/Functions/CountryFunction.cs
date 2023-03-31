using System.Threading.Tasks;
using LocationService.API.Output.Base;
using LocationService.Message.DataTransfer.Countries.v1;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace LocationService.API.Output.Functions;

public class CountryFunction : CountryOutput
{
    public CountryFunction(IMediator mediator) : base(mediator, OutputType.AzureFunction)
    {
    }
    
    [Function($"Country-{nameof(GetAll)}")]
    public async Task<HttpResponseData> GetAll([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/countries")] HttpRequestData req) 
        => await GetAllAsync(req) as HttpResponseData;

    [Function($"Country-{nameof(Get)}")]
    public async Task<HttpResponseData> Get([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/country/{id}")] HttpRequestData req, string id) 
        => await GetAsync(id, req) as HttpResponseData;

    [Function($"Country-{nameof(Create)}")]
    public async Task<HttpResponseData> Create([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/country")] HttpRequestData req) 
        => await CreateAsync(await req.ReadFromJsonAsync<CountryData>(), req) as HttpResponseData;
    
    [Function($"Country-{nameof(Update)}")]
    public async Task<HttpResponseData> Update([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "v1/country")] HttpRequestData req)
        => await UpdateAsync(await req.ReadFromJsonAsync<CountryData>(), req) as HttpResponseData;
    
    [Function($"Country-{nameof(Disable)}")]
    public async Task<HttpResponseData> Disable([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/country/disable/{id}" )] HttpRequestData req, string id)
        => await DisableAsync(id, req) as HttpResponseData;
    
    [Function($"Country-{nameof(Delete)}")]
    public async Task<HttpResponseData> Delete([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/country/{id}")] HttpRequestData req, string id)
        => await DeleteAsync(id, req) as HttpResponseData;
}