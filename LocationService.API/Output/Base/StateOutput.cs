using System.Net;
using System.Threading.Tasks;
using LocationService.Message.DataTransfer.States.v1;
using LocationService.Message.Definition.States.Requests.v1;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker.Http;

namespace LocationService.API.Output.Base;

public class StateOutput : OutputBase
{
    private readonly IMediator _mediator;
    
    public StateOutput(IMediator mediator, OutputType outputType) : base(outputType)
    {
        _mediator = mediator;
    }

    [NonAction]
    protected async Task<object> GetAllAsync(string countryId, HttpRequestData httpRequestData = null)
    {
        var result = await _mediator.Send(new GetAllStates
        {
            CountryId = countryId
        });
        return await TransformToOutputAsync(result, HttpStatusCode.OK, httpRequestData);
    }
    
    [NonAction]
    protected async Task<object> GetAsync(string id, HttpRequestData httpRequestData = null)
    {
        var query = new GetStateById
        {
            Id = id
        };
        
        var result = await _mediator.Send(query);
        return await TransformToOutputAsync(result, result == null ? HttpStatusCode.NotFound : HttpStatusCode.OK, httpRequestData);
    }
    
    [NonAction]
    protected async Task<object> CreateAsync(StateData data, HttpRequestData httpRequestData = null)
    {
        var query = new CreateState
        {
            LocationDetails = data
        };
        
        var result = await _mediator.Send(query);
        return await TransformToOutputAsync(result, result == null ? HttpStatusCode.Conflict : HttpStatusCode.OK, httpRequestData);
    }
    
    [NonAction]
    protected async Task<object> UpdateAsync(StateData data, HttpRequestData httpRequestData = null)
    {
        var query = new UpdateState
        {
            LocationDetails = data
        };
        
        var result = await _mediator.Send(query);
        return await TransformToOutputAsync(result, HttpStatusCode.OK, httpRequestData);
    }
    
    [NonAction]
    protected async Task<object> DisableAsync(string id, HttpRequestData httpRequestData = null)
    {
        var query = new SoftDeleteState
        {
            Id = id
        };
        
        await _mediator.Send(query);
        return await TransformToOutputAsync(null, HttpStatusCode.NoContent, httpRequestData);
    }
    
    [NonAction]
    protected async Task<object> DeleteAsync(string id, HttpRequestData httpRequestData = null)
    {
        var query = new DeleteState
        {
            Id = id
        };
        
        await _mediator.Send(query);
        return await TransformToOutputAsync(null, HttpStatusCode.NoContent, httpRequestData);
    }
}