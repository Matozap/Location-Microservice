using System.Threading.Tasks;
using LocationService.Message.DTO.Countries.v1;
using LocationService.Message.Messaging.Request.Countries.v1;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LocationService.API.Controllers;

[Produces("application/json")]
[Route("api/[controller]")]
[ApiController]
public class CountryController : ControllerBase
{
    private readonly IMediator _mediator;
        
    public CountryController(IMediator mediator)
    {
        _mediator = mediator;
    }
                
    /// <summary>
    ///  - Gets all the countries in the system.
    /// </summary>
    /// <returns>All countries</returns>
    [HttpGet("")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _mediator.Send(new GetAllCountries()));
    }

    /// <summary>
    /// - Gets a country by id (string).
    /// </summary>
    /// <param name="id">Country Id</param>
    /// <returns>Country</returns>        
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var query = new GetCountryById
        {
            CountryId = id
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }
    
    /// <summary>
    /// Creates an country based in the given object. 
    /// </summary>
    /// <param name="country">CountryDto</param>
    /// <returns>Country</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CountryData country)
    {
        var query = new CreateCountry
        {
            LocationDetails = country
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }


    /// <summary>
    /// Updates an country based in the given object.
    /// </summary>
    /// <param name="country">CountryDto</param>
    /// <returns>Country</returns>
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] CountryData country)
    {
        var query = new UpdateCountry
        {
            LocationDetails = country
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Does a soft delete on the country with the given id and returns "Success" if no exception was raised.
    /// </summary>
    /// <param name="id">Country Id</param>
    [HttpDelete("Disable/{id}")]
    public async Task<IActionResult> SoftDelete(string id)
    {
        var query = new SoftDeleteCountry
        {
            CountryId = id
        };
        await _mediator.Send(query);
        return Ok("Success");
    }

    /// <summary>
    /// Does a physical delete on the country with the given id
    /// </summary>
    /// <param name="id">Country Id</param>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var query = new DeleteCountry
        {
            CountryId = id
        };
        await _mediator.Send(query);
        return Ok("Success");
    }
}
