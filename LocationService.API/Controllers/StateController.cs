using System.Threading.Tasks;
using LocationService.Message.DataTransfer.States.v1;
using LocationService.Message.Definition.States.Requests.v1;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LocationService.API.Controllers;

[Produces("application/json")]
[Route("api/v1/")]
[ApiController]
public class StateController : ControllerBase
{
    private readonly IMediator _mediator;
        
    public StateController(IMediator mediator)
    {
        _mediator = mediator;
    }
                
    /// <summary>
    /// Gets all the states in a country.
    /// </summary>
    /// <returns>All States</returns>
    /// <response code="200">OK</response>
    /// <response code="500">Internal Server error</response>
    [HttpGet("states/{countryId}")]
    public async Task<IActionResult> GetAll(string countryId)
    {
        var query = new GetAllStates
        {
            CountryId = countryId
        };
        
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Gets s state by id (number or string).
    /// </summary>
    /// <param name="code">State Id or code</param>
    /// <returns>State</returns>
    /// <response code="200">OK</response>
    /// <response code="404">Not Found</response>
    /// <response code="500">Internal Server error</response>
    [HttpGet("state/{code}")]
    public async Task<IActionResult> Get(string code)
    {
        var query = new GetStateById
        {
            Id = code,
            Code = code
        };
        
        var result = await _mediator.Send(query);

        return result == null ? NotFound() :
            Ok(result);
    }
    
    /// <summary>
    /// Creates an state based in the given object. 
    /// </summary>
    /// <param name="data">State Data</param>
    /// <returns>State</returns>
    /// <response code="201">Created</response>
    /// <response code="409">Conflict</response>
    /// <response code="500">Internal Server error</response>
    [HttpPost("state")]
    public async Task<IActionResult> Create([FromBody] StateData data)
    {
        var query = new CreateState
        {
            LocationDetails = data
        };
        
        var result = await _mediator.Send(query);
        return result == null ? Conflict() : 
            CreatedAtAction("Create", new {id = ((StateData)result).Id}, result);
    }


    /// <summary>
    /// Updates an state based in the given object.
    /// </summary>
    /// <param name="data">State Data</param>
    /// <returns>Country</returns>
    /// <response code="200">OK</response>
    /// <response code="500">Internal Server error</response>
    [HttpPut("state")]
    public async Task<IActionResult> Update([FromBody] StateData data)
    {
        var query = new UpdateState
        {
            LocationDetails = data
        };
        
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Does a soft delete on the state with the given id.
    /// </summary>
    /// <param name="id">Id</param>
    /// <response code="204">No Content</response>
    /// <response code="500">Internal Server error</response>
    [HttpDelete("state/disable/{id}")]
    public async Task<IActionResult> Disable(string id)
    {
        var query = new SoftDeleteState
        {
            Id = id
        };
        
        await _mediator.Send(query);
        return NoContent();
    }

    /// <summary>
    /// Does a physical delete on the state with the given id.
    /// </summary>
    /// <param name="id">Id</param>
    /// <response code="204">No Content</response>
    /// <response code="500">Internal Server error</response>
    [HttpDelete("state/{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var query = new DeleteState
        {
            Id = id
        };
        
        await _mediator.Send(query);
        return NoContent();
    }
}
