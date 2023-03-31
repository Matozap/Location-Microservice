using System.Threading.Tasks;
using LocationService.API.Output.Base;
using LocationService.Message.DataTransfer.Cities.v1;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LocationService.API.Output.Controllers;

[Produces("application/json")]
[Route("api/v1/")]
[ApiController]
public class CityController : CityOutput
{
    public CityController(IMediator mediator) : base(mediator, OutputType.Controller)
    {
    }
                
    /// <summary>
    /// Gets all the cities in a state.
    /// </summary>
    /// <returns>All Cities</returns>
    /// <response code="200">OK</response>
    /// <response code="500">Internal Server error</response>
    [HttpGet("cities/{stateId}")]
    public async Task<IActionResult> GetAll(string stateId) => await GetAllAsync(stateId) as IActionResult;

    /// <summary>
    /// Gets a city by id (number or string).
    /// </summary>
    /// <param name="id">Id</param>
    /// <returns>Country</returns>
    /// <response code="200">OK</response>
    /// <response code="404">Not Found</response>
    /// <response code="500">Internal Server error</response>
    [HttpGet("city/{id}")]
    public async Task<IActionResult> Get(string id) => await GetAsync(id) as IActionResult;
    
    /// <summary>
    /// Creates an city based in the given object. 
    /// </summary>
    /// <param name="data">City Data</param>
    /// <returns>City</returns>
    /// <response code="201">Created</response>
    /// <response code="409">Conflict</response>
    /// <response code="500">Internal Server error</response>
    [HttpPost("city")]
    public async Task<IActionResult> Create([FromBody] CityData data) => await CreateAsync(data) as IActionResult;

    /// <summary>
    /// Updates an city based in the given object.
    /// </summary>
    /// <param name="data">City Data</param>
    /// <returns>Country</returns>
    /// <response code="200">OK</response>
    /// <response code="500">Internal Server error</response>
    [HttpPut("city")]
    public async Task<IActionResult> Update([FromBody] CityData data) => await UpdateAsync(data) as IActionResult;

    /// <summary>
    /// Does a soft delete on the city with the given id.
    /// </summary>
    /// <param name="id">Id</param>
    /// <response code="204">No Content</response>
    /// <response code="500">Internal Server error</response>
    [HttpDelete("city/disable/{id}")]
    public async Task<IActionResult> Disable(string id) => await DisableAsync(id) as IActionResult;

    /// <summary>
    /// Does a physical delete on the city with the given id.
    /// </summary>
    /// <param name="id">Id</param>
    /// <response code="204">No Content</response>
    /// <response code="500">Internal Server error</response>
    [HttpDelete("city/{id}")]
    public async Task<IActionResult> Delete(string id) => await DeleteAsync(id) as IActionResult;
}
