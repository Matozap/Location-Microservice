using System.Threading.Tasks;
using LocationService.API.Output.Base;
using LocationService.Message.DataTransfer.States.v1;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LocationService.API.Output.Controllers;

[Produces("application/json")]
[Route("api/v1/")]
[ApiController]
public class StateController : StateOutput
{
    public StateController(IMediator mediator) : base(mediator, OutputType.Controller)
    {
    }

    /// <summary>
    /// Gets all the states in a country.
    /// </summary>
    /// <returns>All States</returns>
    /// <response code="200">OK</response>
    /// <response code="500">Internal Server error</response>
    [HttpGet("states/{countryId}")]
    public async Task<IActionResult> GetAll(string countryId) => await GetAllAsync(countryId) as IActionResult;

    /// <summary>
    /// Gets s state by id (number or string).
    /// </summary>
    /// <param name="code">State Id or code</param>
    /// <returns>State</returns>
    /// <response code="200">OK</response>
    /// <response code="404">Not Found</response>
    /// <response code="500">Internal Server error</response>
    [HttpGet("state/{code}")]
    public async Task<IActionResult> Get(string code) => await GetAsync(code) as IActionResult;
    
    /// <summary>
    /// Creates an state based in the given object. 
    /// </summary>
    /// <param name="data">State Data</param>
    /// <returns>State</returns>
    /// <response code="201">Created</response>
    /// <response code="409">Conflict</response>
    /// <response code="500">Internal Server error</response>
    [HttpPost("state")]
    public async Task<IActionResult> Create([FromBody] StateData data) => await CreateAsync(data) as IActionResult;

    /// <summary>
    /// Updates an state based in the given object.
    /// </summary>
    /// <param name="data">State Data</param>
    /// <returns>Country</returns>
    /// <response code="200">OK</response>
    /// <response code="500">Internal Server error</response>
    [HttpPut("state")]
    public async Task<IActionResult> Update([FromBody] StateData data) => await UpdateAsync(data) as IActionResult;

    /// <summary>
    /// Does a soft delete on the state with the given id.
    /// </summary>
    /// <param name="id">Id</param>
    /// <response code="204">No Content</response>
    /// <response code="500">Internal Server error</response>
    [HttpDelete("state/disable/{id}")]
    public async Task<IActionResult> Disable(string id) => await DisableAsync(id) as IActionResult;

    /// <summary>
    /// Does a physical delete on the state with the given id.
    /// </summary>
    /// <param name="id">Id</param>
    /// <response code="204">No Content</response>
    /// <response code="500">Internal Server error</response>
    [HttpDelete("state/{id}")]
    public async Task<IActionResult> Delete(string id) => await DeleteAsync(id) as IActionResult;
}
