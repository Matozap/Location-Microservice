using System.Net;
using System.Threading.Tasks;
using LocationService.Message.DataTransfer.Countries.v1;
using LocationService.Message.Definition.Countries.Requests.v1;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace LocationService.API.Functions;

public class CountryFunction
{
    private readonly IMediator _mediator;

    public CountryFunction(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [Function($"Country-{nameof(GetAll)}")]
    public async Task<HttpResponseData> GetAll([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/countries")] HttpRequestData req)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(await _mediator.Send(new GetAllCountries()));
        return response;
    }
    
    [Function($"Country-{nameof(Get)}")]
    public async Task<HttpResponseData> Get([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/country/{id}")] HttpRequestData req, string id)
    {
        var query = new GetCountryById
        {
            Id = id
        };
        
        var result = await _mediator.Send(query);
        
        if (result == null)
        {
            return req.CreateResponse(HttpStatusCode.NotFound); 
        }

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(result);
        return response;
    }
    
    [Function($"Country-{nameof(Create)}")]
    public async Task<HttpResponseData> Create([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/country")] HttpRequestData req)
    {
        var data = await req.ReadFromJsonAsync<CountryData>();
        
        var query = new CreateCountry
        {
            LocationDetails = data
        };
        
        var result = await _mediator.Send(query);

        if (result == null)
        {
            return req.CreateResponse(HttpStatusCode.Conflict); 
        }
        
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(result);
        return response;
    }
    
    [Function($"Country-{nameof(Update)}")]
    public async Task<HttpResponseData> Update([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "v1/country")] HttpRequestData req)
    {
        var data = await req.ReadFromJsonAsync<CountryData>();
        
        var query = new UpdateCountry
        {
            LocationDetails = data
        };
        
        var result = await _mediator.Send(query);

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(result);
        return response;
    }
    
    [Function($"Country-{nameof(Disable)}")]
    public async Task<HttpResponseData> Disable([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/country/disable/{id}" )] HttpRequestData req, string id)
    {
        var query = new SoftDeleteCountry
        {
            Id = id
        };
        
        await _mediator.Send(query);

        var response = req.CreateResponse(HttpStatusCode.NoContent);
        return response;
    }
    
    [Function($"Country-{nameof(Delete)}")]
    public async Task<HttpResponseData> Delete([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/country/{id}")] HttpRequestData req, string id)
    {
        var query = new DeleteCountry
        {
            Id = id
        };
        
        await _mediator.Send(query);

        var response = req.CreateResponse(HttpStatusCode.NoContent);
        return response;
    }
}