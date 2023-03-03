using System.Threading.Tasks;
using LocationService.Message.DTO.States.v1;
using LocationService.Message.Messaging.Request.States.v1;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LocationService.API.Controllers;

[Produces("application/json")]
[Route("api/[controller]")]
[ApiController]
public class StateController : ControllerBase
{
    private readonly IMediator _mediator;
        
    public StateController(IMediator mediator)
    {
        _mediator = mediator;
    }
                
    /// <summary>
    ///  - Gets all the states in the system.
    /// </summary>
    /// <returns>All States</returns>
    [HttpGet("all/{countryId}")]
    public async Task<IActionResult> GetAll(string countryId)
    {
        var query = new GetAllStates()
        {
            CountryId = countryId
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// - Gets s State by id (number or string).
    /// </summary>
    /// <param name="idCode">State Id or code</param>
    /// <returns>Country</returns>        
    [HttpGet("{idCode}")]
    public async Task<IActionResult> GetById(string idCode)
    {
        var isNumber = int.TryParse(idCode, out var parsedId);
        var query = new GetStateById
        {
            Id = isNumber ? parsedId : 0,
            Code = idCode
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }
    
    /// <summary>
    /// Creates an state based in the given object. 
    /// </summary>
    /// <param name="location">CountryDto</param>
    /// <returns>State</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] StateData location)
    {
        var query = new CreateState
        {
            LocationDetails = location
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }


    /// <summary>
    /// Updates an state based in the given object.
    /// </summary>
    /// <param name="location">CountryDto</param>
    /// <returns>Country</returns>
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] StateData location)
    {
        var query = new UpdateState
        {
            LocationDetails = location
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Does a soft delete on the State with the given id and returns "Success" if no exception was raised.
    /// </summary>
    /// <param name="id">Country Id</param>
    [HttpDelete("Disable/{id}")]
    public async Task<IActionResult> SoftDelete(int id)
    {
        var query = new SoftDeleteState
        {
            StateId = id
        };
        await _mediator.Send(query);
        return Ok("Success");
    }

    /// <summary>
    /// Does a physical delete on the location with the given id
    /// </summary>
    /// <param name="id">Country Id</param>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var query = new DeleteState
        {
            Id = id
        };
        await _mediator.Send(query);
        return Ok("Success");
    }
}
