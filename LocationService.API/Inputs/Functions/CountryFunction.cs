using System.Threading.Tasks;
using LocationService.API.Outputs;
using LocationService.API.Outputs.Base;
using LocationService.Message.DataTransfer.Countries.v1;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace LocationService.API.Inputs.Functions;

public class CountryFunction
{
    private readonly CountryOutput _countryOutput;
    
    public CountryFunction(IMediator mediator)
    {
        _countryOutput = new CountryOutput(mediator, OutputType.AzureFunction);
    }
    
    [Function($"Country-{nameof(GetAll)}")]
    public async Task<HttpResponseData> GetAll([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/countries")] HttpRequestData req) 
        => await _countryOutput.GetAllAsync<HttpResponseData>(req);

    [Function($"Country-{nameof(Get)}")]
    public async Task<HttpResponseData> Get([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/country/{id}")] HttpRequestData req, string id) 
        => await _countryOutput.GetAsync<HttpResponseData>(id, req);

    [Function($"Country-{nameof(Create)}")]
    public async Task<HttpResponseData> Create([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/country")] HttpRequestData req) 
        => await _countryOutput.CreateAsync<HttpResponseData>(await req.ReadFromJsonAsync<CountryData>(), req);
    
    [Function($"Country-{nameof(Update)}")]
    public async Task<HttpResponseData> Update([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "v1/country")] HttpRequestData req)
        => await _countryOutput.UpdateAsync<HttpResponseData>(await req.ReadFromJsonAsync<CountryData>(), req);
    
    [Function($"Country-{nameof(Disable)}")]
    public async Task<HttpResponseData> Disable([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/country/disable/{id}" )] HttpRequestData req, string id)
        => await _countryOutput.DisableAsync<HttpResponseData>(id, req);
    
    [Function($"Country-{nameof(Delete)}")]
    public async Task<HttpResponseData> Delete([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/country/{id}")] HttpRequestData req, string id)
        => await _countryOutput.DeleteAsync<HttpResponseData>(id, req);
}