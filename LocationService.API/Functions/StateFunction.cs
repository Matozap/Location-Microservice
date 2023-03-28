using System.Net;
using System.Threading.Tasks;
using LocationService.Message.DataTransfer.States.v1;
using LocationService.Message.Definition.States.Requests.v1;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace LocationService.API.Functions;

public class StateFunction
{
    private readonly IMediator _mediator;

    public StateFunction(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [Function($"State-{nameof(GetAll)}")]
    public async Task<HttpResponseData> GetAll([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/states/{countryId}")] HttpRequestData req, string countryId)
    {
        var query = new GetAllStates
        {
            CountryId = countryId
        };
        
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(await _mediator.Send(query));
        return response;
    }
    
    [Function($"State-{nameof(Get)}")]
    public async Task<HttpResponseData> Get([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/state/{id}")] HttpRequestData req, string id)
    {
        var query = new GetStateById
        {
            Id = id,
            Code = id
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
    
    [Function($"State-{nameof(Create)}")]
    public async Task<HttpResponseData> Create([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/state")] HttpRequestData req)
    {
        var data = await req.ReadFromJsonAsync<StateData>();
        
        var query = new CreateState
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
    
    [Function($"State-{nameof(Update)}")]
    public async Task<HttpResponseData> Update([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "v1/state")] HttpRequestData req)
    {
        var data = await req.ReadFromJsonAsync<StateData>();
        
        var query = new UpdateState
        {
            LocationDetails = data
        };
        
        var result = await _mediator.Send(query);

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(result);
        return response;
    }
    
    [Function($"State-{nameof(Disable)}")]
    public async Task<HttpResponseData> Disable([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/state/disable/{id}" )] HttpRequestData req, string id)
    {
        var query = new SoftDeleteState
        {
            Id = id
        };
        
        await _mediator.Send(query);

        var response = req.CreateResponse(HttpStatusCode.NoContent);
        return response;
    }
    
    [Function($"State-{nameof(Delete)}")]
    public async Task<HttpResponseData> Delete([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/state/{id}")] HttpRequestData req, string id)
    {
        var query = new DeleteState
        {
            Id = id
        };
        
        await _mediator.Send(query);

        var response = req.CreateResponse(HttpStatusCode.NoContent);
        return response;
    }
}