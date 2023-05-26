using System.Net;
using System.Threading.Tasks;
using LocationService.API.Outputs.Base;
using LocationService.Message.Cities;
using LocationService.Message.Cities.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker.Http;

namespace LocationService.API.Outputs;

public class CityOutput : OutputBase
{
    private readonly IMediator _mediator;
    
    public CityOutput(IMediator mediator, OutputType outputType) : base(outputType)
    {
        _mediator = mediator;
    }

    [NonAction]
    public async Task<T> GetAllAsync<T>(string stateId, HttpRequestData httpRequestData = null) where T: class
    {
        var result = await _mediator.Send(new GetAllCities
        {
            StateId = stateId
        });
        return await TransformToOutputAsync(result, HttpStatusCode.OK, httpRequestData) as T;
    }
    
    [NonAction]
    public async Task<T> GetAsync<T>(string id, HttpRequestData httpRequestData = null) where T: class
    {
        var query = new GetCityById
        {
            Id = id
        };
        
        var result = await _mediator.Send(query);
        return await TransformToOutputAsync(result, result == null ? HttpStatusCode.NotFound : HttpStatusCode.OK, httpRequestData) as T;
    }
    
    [NonAction]
    public async Task<T> CreateAsync<T>(CityData data, HttpRequestData httpRequestData = null) where T: class
    {
        var query = new CreateCity
        {
            Details = data
        };
        
        var result = await _mediator.Send(query);
        return await TransformToOutputAsync(result, result == null ? HttpStatusCode.Conflict : HttpStatusCode.OK, httpRequestData) as T;
    }
    
    [NonAction]
    public async Task<T> UpdateAsync<T>(CityData data, HttpRequestData httpRequestData = null) where T: class
    {
        var query = new UpdateCity
        {
            Details = data
        };
        
        var result = await _mediator.Send(query);
        return await TransformToOutputAsync(result, result == null ? HttpStatusCode.NotFound : HttpStatusCode.OK, httpRequestData) as T;
    }
    
    [NonAction]
    public async Task<T> DisableAsync<T>(string id, HttpRequestData httpRequestData = null) where T: class
    {
        var query = new SoftDeleteCity
        {
            Id = id
        };
        
        await _mediator.Send(query);
        return await TransformToOutputAsync(null, HttpStatusCode.NoContent, httpRequestData) as T;
    }
    
    [NonAction]
    public async Task<T> DeleteAsync<T>(string id, HttpRequestData httpRequestData = null) where T: class
    {
        var query = new DeleteCity
        {
            Id = id
        };
        
        await _mediator.Send(query);
        return await TransformToOutputAsync(null, HttpStatusCode.NoContent, httpRequestData) as T;
    }
}