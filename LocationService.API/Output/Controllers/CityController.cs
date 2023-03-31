using System.Threading.Tasks;
using LocationService.Message.DataTransfer.Cities.v1;
using LocationService.Message.Definition.Cities.Requests.v1;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LocationService.API.Output.Controllers;

[Produces("application/json")]
[Route("api/v1/")]
[ApiController]
public class CityController : ControllerBase
{
    private readonly IMediator _mediator;
        
    public CityController(IMediator mediator)
    {
        _mediator = mediator;
    }
                
    /// <summary>
    /// Gets all the cities in a state.
    /// </summary>
    /// <returns>All Cities</returns>
    /// <response code="200">OK</response>
    /// <response code="500">Internal Server error</response>
    [HttpGet("cities/{stateId}")]
    public async Task<IActionResult> GetAll(string stateId)
    {
        var query = new GetAllCities
        {
            StateId = stateId
        };
        
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Gets a city by id (number or string).
    /// </summary>
    /// <param name="id">Id</param>
    /// <returns>Country</returns>
    /// <response code="200">OK</response>
    /// <response code="404">Not Found</response>
    /// <response code="500">Internal Server error</response>
    [HttpGet("city/{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var query = new GetCityById
        {
            Id = id
        };
        
        var result = await _mediator.Send(query);

        return result == null ? NotFound() :
            Ok(result);
    }
    
    /// <summary>
    /// Creates an city based in the given object. 
    /// </summary>
    /// <param name="data">City Data</param>
    /// <returns>City</returns>
    /// <response code="201">Created</response>
    /// <response code="409">Conflict</response>
    /// <response code="500">Internal Server error</response>
    [HttpPost("city")]
    public async Task<IActionResult> Create([FromBody] CityData data)
    {
        var query = new CreateCity
        {
            LocationDetails = data
        };
        
        var result = await _mediator.Send(query);
        return result == null ? Conflict() : 
            CreatedAtAction("Create", new {id = ((CityData)result).Id}, result);
    }


    /// <summary>
    /// Updates an city based in the given object.
    /// </summary>
    /// <param name="data">City Data</param>
    /// <returns>Country</returns>
    /// <response code="200">OK</response>
    /// <response code="500">Internal Server error</response>
    [HttpPut("city")]
    public async Task<IActionResult> Update([FromBody] CityData data)
    {
        var query = new UpdateCity
        {
            LocationDetails = data
        };
        
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Does a soft delete on the city with the given id.
    /// </summary>
    /// <param name="id">Id</param>
    /// <response code="204">No Content</response>
    /// <response code="500">Internal Server error</response>
    [HttpDelete("city/disable/{id}")]
    public async Task<IActionResult> Disable(string id)
    {
        var query = new SoftDeleteCity
        {
            Id = id
        };
        
        await _mediator.Send(query);
        return NoContent();
    }

    /// <summary>
    /// Does a physical delete on the city with the given id.
    /// </summary>
    /// <param name="id">Id</param>
    /// <response code="204">No Content</response>
    /// <response code="500">Internal Server error</response>
    [HttpDelete("city/{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var query = new DeleteCity
        {
            Id = id
        };
        
        await _mediator.Send(query);
        return NoContent();
    }
}
