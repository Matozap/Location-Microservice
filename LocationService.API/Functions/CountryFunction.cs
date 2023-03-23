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
    
    [Function(nameof(GetAll))]
    public async Task<HttpResponseData> GetAll([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Country")] HttpRequestData req)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(await _mediator.Send(new GetAllCountries()));
        return response;
    }
    
    [Function(nameof(Get))]
    public async Task<HttpResponseData> Get([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Country/{id}")] HttpRequestData req, string id)
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
    
    [Function(nameof(Create))]
    public async Task<HttpResponseData> Create([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Country")] HttpRequestData req)
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
    
    [Function(nameof(Update))]
    public async Task<HttpResponseData> Update([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "Country")] HttpRequestData req)
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
    
    [Function(nameof(Disable))]
    public async Task<HttpResponseData> Disable([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "Country/Disable/{id}" )] HttpRequestData req, string id)
    {
        var query = new SoftDeleteCountry
        {
            Id = id
        };
        
        await _mediator.Send(query);

        var response = req.CreateResponse(HttpStatusCode.NoContent);
        return response;
    }
    
    [Function(nameof(Delete))]
    public async Task<HttpResponseData> Delete([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "Country/{id}")] HttpRequestData req, string id)
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