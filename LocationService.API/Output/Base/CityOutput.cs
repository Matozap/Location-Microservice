using System.Net;
using System.Threading.Tasks;
using LocationService.Message.DataTransfer.Cities.v1;
using LocationService.Message.Definition.Cities.Requests.v1;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker.Http;

namespace LocationService.API.Output.Base;

public class CityOutput : OutputBase
{
    private readonly IMediator _mediator;
    
    public CityOutput(IMediator mediator, OutputType outputType) : base(outputType)
    {
        _mediator = mediator;
    }

    [NonAction]
    protected async Task<object> GetAllAsync(string stateId, HttpRequestData httpRequestData = null)
    {
        var result = await _mediator.Send(new GetAllCities
        {
            StateId = stateId
        });
        return await TransformToOutputAsync(result, HttpStatusCode.OK, httpRequestData);
    }
    
    [NonAction]
    protected async Task<object> GetAsync(string id, HttpRequestData httpRequestData = null)
    {
        var query = new GetCityById
        {
            Id = id
        };
        
        var result = await _mediator.Send(query);
        return await TransformToOutputAsync(result, result == null ? HttpStatusCode.NotFound : HttpStatusCode.OK, httpRequestData);
    }
    
    [NonAction]
    protected async Task<object> CreateAsync(CityData data, HttpRequestData httpRequestData = null)
    {
        var query = new CreateCity
        {
            Details = data
        };
        
        var result = await _mediator.Send(query);
        return await TransformToOutputAsync(result, result == null ? HttpStatusCode.Conflict : HttpStatusCode.OK, httpRequestData);
    }
    
    [NonAction]
    protected async Task<object> UpdateAsync(CityData data, HttpRequestData httpRequestData = null)
    {
        var query = new UpdateCity
        {
            Details = data
        };
        
        var result = await _mediator.Send(query);
        return await TransformToOutputAsync(result, HttpStatusCode.OK, httpRequestData);
    }
    
    [NonAction]
    protected async Task<object> DisableAsync(string id, HttpRequestData httpRequestData = null)
    {
        var query = new SoftDeleteCity
        {
            Id = id
        };
        
        await _mediator.Send(query);
        return await TransformToOutputAsync(null, HttpStatusCode.NoContent, httpRequestData);
    }
    
    [NonAction]
    protected async Task<object> DeleteAsync(string id, HttpRequestData httpRequestData = null)
    {
        var query = new DeleteCity
        {
            Id = id
        };
        
        await _mediator.Send(query);
        return await TransformToOutputAsync(null, HttpStatusCode.NoContent, httpRequestData);
    }
}