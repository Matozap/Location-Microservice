using System.Threading.Tasks;
using LocationService.Message.DataTransfer.Countries.v1;
using LocationService.Message.Definition.Countries.Requests.v1;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LocationService.API.Controllers;

[Produces("application/json")]
[Route("api/v1/")]
[ApiController]
public class CountryController : ControllerBase
{
    private readonly IMediator _mediator;

    public CountryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets all the countries in the system.
    /// </summary>
    /// <returns>All countries</returns>
    /// <response code="200">OK</response>
    /// <response code="500">Internal Server error</response>
    [HttpGet("countries")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _mediator.Send(new GetAllCountries()));
    }

    /// <summary>
    /// Gets a country by id (string).
    /// </summary>
    /// <param name="id">Id or Code</param>
    /// <returns>Country</returns>
    /// <response code="200">OK</response>
    /// <response code="404">Not Found</response>
    /// <response code="500">Internal Server error</response>
    [HttpGet("country/{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var query = new GetCountryById
        {
            Id = id
        };
        
        var result = await _mediator.Send(query);

        return result == null ? NotFound() :
            Ok(result);
    }

    /// <summary>
    /// Creates an country based in the given object. 
    /// </summary>
    /// <param name="data">Country Data</param>
    /// <returns>Country</returns>
    /// <response code="201">Created</response>
    /// <response code="409">Conflict</response>
    /// <response code="500">Internal Server error</response>
    [HttpPost("country")]
    public async Task<IActionResult> Create([FromBody] CountryData data)
    {
        var query = new CreateCountry
        {
            LocationDetails = data
        };
        
        var result = await _mediator.Send(query);
        return result == null ? Conflict() : 
            CreatedAtAction("Create", new {id = ((CountryData)result).Id}, result);
    }

    /// <summary>
    /// Updates an country based in the given object.
    /// </summary>
    /// <param name="data">Data</param>
    /// <returns>Country</returns>
    /// <response code="200">OK</response>
    /// <response code="500">Internal Server error</response>
    [HttpPut("country")]
    public async Task<IActionResult> Update([FromBody] CountryData data)
    {
        var query = new UpdateCountry
        {
            LocationDetails = data
        };
        
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Does a soft delete on the country with the given id.
    /// </summary>
    /// <param name="id">Id or Code</param>
    /// <response code="204">No Content</response>
    /// <response code="500">Internal Server error</response>
    [HttpDelete("country/disable/{id}")]
    public async Task<IActionResult> Disable(string id)
    {
        var query = new SoftDeleteCountry
        {
            Id = id
        };
        
        await _mediator.Send(query);
        return NoContent();
    }

    /// <summary>
    /// Does a physical delete on the country with the given id.
    /// </summary>
    /// <param name="id">Id or Code</param>
    /// <response code="204">No Content</response>
    /// <response code="500">Internal Server error</response>
    [HttpDelete("country/{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var query = new DeleteCountry
        {
            Id = id
        };
        
        await _mediator.Send(query);
        return NoContent();
    }
}