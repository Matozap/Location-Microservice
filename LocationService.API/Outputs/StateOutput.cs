﻿using System.Net;
using System.Threading.Tasks;
using LocationService.API.Outputs.Base;
using LocationService.Message.DataTransfer.States.v1;
using LocationService.Message.Definition.States.Requests.v1;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker.Http;

namespace LocationService.API.Outputs;

public class StateOutput : OutputBase
{
    private readonly IMediator _mediator;
    
    public StateOutput(IMediator mediator, OutputType outputType) : base(outputType)
    {
        _mediator = mediator;
    }

    [NonAction]
    protected async Task<T> GetAllAsync<T>(string countryId, HttpRequestData httpRequestData = null) where T: class
    {
        var result = await _mediator.Send(new GetAllStates
        {
            CountryId = countryId
        });
        return await TransformToOutputAsync(result, HttpStatusCode.OK, httpRequestData) as T;
    }
    
    [NonAction]
    protected async Task<T> GetAsync<T>(string code, HttpRequestData httpRequestData = null) where T: class
    {
        var query = new GetStateById
        {
            Id = code,
            Code = code
        };
        
        var result = await _mediator.Send(query);
        return await TransformToOutputAsync(result, result == null ? HttpStatusCode.NotFound : HttpStatusCode.OK, httpRequestData) as T;
    }
    
    [NonAction]
    protected async Task<T> CreateAsync<T>(StateData data, HttpRequestData httpRequestData = null) where T: class
    {
        var query = new CreateState
        {
            Details = data
        };
        
        var result = await _mediator.Send(query);
        return await TransformToOutputAsync(result, result == null ? HttpStatusCode.Conflict : HttpStatusCode.OK, httpRequestData) as T;
    }
    
    [NonAction]
    protected async Task<T> UpdateAsync<T>(StateData data, HttpRequestData httpRequestData = null) where T: class
    {
        var query = new UpdateState
        {
            Details = data
        };
        
        var result = await _mediator.Send(query);
        return await TransformToOutputAsync(result, HttpStatusCode.OK, httpRequestData) as T;
    }
    
    [NonAction]
    protected async Task<T> DisableAsync<T>(string id, HttpRequestData httpRequestData = null) where T: class
    {
        var query = new SoftDeleteState
        {
            Id = id
        };
        
        await _mediator.Send(query);
        return await TransformToOutputAsync(null, HttpStatusCode.NoContent, httpRequestData) as T;
    }
    
    [NonAction]
    protected async Task<T> DeleteAsync<T>(string id, HttpRequestData httpRequestData = null) where T: class
    {
        var query = new DeleteState
        {
            Id = id
        };
        
        await _mediator.Send(query);
        return await TransformToOutputAsync(null, HttpStatusCode.NoContent, httpRequestData) as T;
    }
}