using System.Threading.Tasks;
using LocationService.API.Outputs;
using LocationService.API.Outputs.Base;
using LocationService.Message.Countries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LocationService.API.Inputs.Controllers;

[Produces("application/json")]
[Route("api/v1/")]
[ApiController]
public class CountryController : ControllerBase
{
    private readonly CountryOutput _countryOutput;
    
    public CountryController(IMediator mediator)
    {
        _countryOutput = new CountryOutput(mediator, OutputType.Controller);
    }

    /// <summary>
    /// Gets all the countries in the system.
    /// </summary>
    /// <returns>All countries</returns>
    /// <response code="200">OK</response>
    /// <response code="500">Internal Server error</response>
    [HttpGet("countries")]
    public async Task<IActionResult> GetAll() => await _countryOutput.GetAllAsync<ActionResult>();

    /// <summary>
    /// Gets a country by id (string).
    /// </summary>
    /// <param name="id">Id or Code</param>
    /// <returns>Country</returns>
    /// <response code="200">OK</response>
    /// <response code="404">Not Found</response>
    /// <response code="500">Internal Server error</response>
    [HttpGet("country/{id}")]
    public async Task<IActionResult> Get(string id) => await _countryOutput.GetAsync<ActionResult>(id);

    /// <summary>
    /// Creates an country based in the given object. 
    /// </summary>
    /// <param name="data">Country Data</param>
    /// <returns>Country</returns>
    /// <response code="201">Created</response>
    /// <response code="409">Conflict</response>
    /// <response code="500">Internal Server error</response>
    [HttpPost("country")]
    public async Task<IActionResult> Create([FromBody] CountryData data) => await _countryOutput.CreateAsync<ActionResult>(data);

    /// <summary>
    /// Updates an country based in the given object.
    /// </summary>
    /// <param name="data">Data</param>
    /// <returns>Country</returns>
    /// <response code="200">OK</response>
    /// <response code="500">Internal Server error</response>
    [HttpPut("country")]
    public async Task<IActionResult> Update([FromBody] CountryData data) => await _countryOutput.UpdateAsync<ActionResult>(data);

    /// <summary>
    /// Does a soft delete on the country with the given id.
    /// </summary>
    /// <param name="id">Id or Code</param>
    /// <response code="204">No Content</response>
    /// <response code="500">Internal Server error</response>
    [HttpDelete("country/disable/{id}")]
    public async Task<IActionResult> Disable(string id) => await _countryOutput.DisableAsync<ActionResult>(id);

    /// <summary>
    /// Does a physical delete on the country with the given id.
    /// </summary>
    /// <param name="id">Id or Code</param>
    /// <response code="204">No Content</response>
    /// <response code="500">Internal Server error</response>
    [HttpDelete("country/{id}")]
    public async Task<IActionResult> Delete(string id) => await _countryOutput.DeleteAsync<ActionResult>(id);
}