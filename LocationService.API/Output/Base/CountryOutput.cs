using System.Net;
using System.Threading.Tasks;
using LocationService.Message.DataTransfer.Countries.v1;
using LocationService.Message.Definition.Countries.Requests.v1;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker.Http;

namespace LocationService.API.Output.Base;

public class CountryOutput : OutputBase
{
    private readonly IMediator _mediator;
    
    public CountryOutput(IMediator mediator, OutputType outputType) : base(outputType)
    {
        _mediator = mediator;
    }

    [NonAction]
    protected async Task<object> GetAllAsync(HttpRequestData httpRequestData = null)
    {
        var result = await _mediator.Send(new GetAllCountries());
        return await TransformToOutputAsync(result, HttpStatusCode.OK, httpRequestData);
    }
    
    [NonAction]
    protected async Task<object> GetAsync(string id, HttpRequestData httpRequestData = null)
    {
        var query = new GetCountryById
        {
            Id = id
        };
        
        var result = await _mediator.Send(query);
        return await TransformToOutputAsync(result, result == null ? HttpStatusCode.NotFound : HttpStatusCode.OK, httpRequestData);
    }
    
    [NonAction]
    protected async Task<object> CreateAsync(CountryData data, HttpRequestData httpRequestData = null)
    {
        var query = new CreateCountry
        {
            Details = data
        };
        
        var result = await _mediator.Send(query);
        return await TransformToOutputAsync(result, result == null ? HttpStatusCode.Conflict : HttpStatusCode.OK, httpRequestData);
    }
    
    [NonAction]
    protected async Task<object> UpdateAsync(CountryData data, HttpRequestData httpRequestData = null)
    {
        var query = new UpdateCountry
        {
            Details = data
        };
        
        var result = await _mediator.Send(query);
        return await TransformToOutputAsync(result, HttpStatusCode.OK, httpRequestData);
    }
    
    [NonAction]
    protected async Task<object> DisableAsync(string id, HttpRequestData httpRequestData = null)
    {
        var query = new SoftDeleteCountry
        {
            Id = id
        };
        
        await _mediator.Send(query);
        return await TransformToOutputAsync(null, HttpStatusCode.NoContent, httpRequestData);
    }
    
    [NonAction]
    protected async Task<object> DeleteAsync(string id, HttpRequestData httpRequestData = null)
    {
        var query = new DeleteCountry
        {
            Id = id
        };
        
        await _mediator.Send(query);
        return await TransformToOutputAsync(null, HttpStatusCode.NoContent, httpRequestData);
    }
}