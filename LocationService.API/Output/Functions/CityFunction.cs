using System.Net;
using System.Threading.Tasks;
using LocationService.Message.DataTransfer.Cities.v1;
using LocationService.Message.Definition.Cities.Requests.v1;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace LocationService.API.Output.Functions;

public class CityFunction
{
    private readonly IMediator _mediator;

    public CityFunction(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [Function($"City-{nameof(GetAll)}")]
    public async Task<HttpResponseData> GetAll([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/cities/{stateId}")] HttpRequestData req, string stateId)
    {
        var query = new GetAllCities
        {
            StateId = stateId
        };
        
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(await _mediator.Send(query));
        return response;
    }
    
    [Function($"City-{nameof(Get)}")]
    public async Task<HttpResponseData> Get([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/city/{id}")] HttpRequestData req, string id)
    {
        var query = new GetCityById
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
    
    [Function($"City-{nameof(Create)}")]
    public async Task<HttpResponseData> Create([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/city")] HttpRequestData req)
    {
        var data = await req.ReadFromJsonAsync<CityData>();
        
        var query = new CreateCity
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
    
    [Function($"City-{nameof(Update)}")]
    public async Task<HttpResponseData> Update([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "v1/city")] HttpRequestData req)
    {
        var data = await req.ReadFromJsonAsync<CityData>();
        
        var query = new UpdateCity
        {
            LocationDetails = data
        };
        
        var result = await _mediator.Send(query);

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(result);
        return response;
    }
    
    [Function($"City-{nameof(Disable)}")]
    public async Task<HttpResponseData> Disable([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/city/disable/{id}" )] HttpRequestData req, string id)
    {
        var query = new SoftDeleteCity
        {
            Id = id
        };
        
        await _mediator.Send(query);

        var response = req.CreateResponse(HttpStatusCode.NoContent);
        return response;
    }
    
    [Function($"City-{nameof(Delete)}")]
    public async Task<HttpResponseData> Delete([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/city/{id}")] HttpRequestData req, string id)
    {
        var query = new DeleteCity
        {
            Id = id
        };
        
        await _mediator.Send(query);

        var response = req.CreateResponse(HttpStatusCode.NoContent);
        return response;
    }
}