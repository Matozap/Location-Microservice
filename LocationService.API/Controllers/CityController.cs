using System.Threading.Tasks;
using LocationService.Message.DTO.Cities.v1;
using LocationService.Message.Messaging.Request.Cities.v1;
using LocationService.Message.Messaging.Request.v1;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LocationService.API.Controllers;

[Produces("application/json")]
[Route("api/[controller]")]
[ApiController]
public class CityController : ControllerBase
{
    private readonly IMediator _mediator;
        
    public CityController(IMediator mediator)
    {
        _mediator = mediator;
    }
                
    /// <summary>
    ///  - Gets all the cities in the system.
    /// </summary>
    /// <returns>All Citys</returns>
    [HttpGet("all/{cityId}")]
    public async Task<IActionResult> GetAll(int cityId)
    {
        var query = new GetAllCities
        {
            StateId = cityId
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// - Gets s City by id (number or string).
    /// </summary>
    /// <param name="id">City Id or code</param>
    /// <returns>Country</returns>        
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new GetCityById
        {
            CityId = id
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }
    
    /// <summary>
    /// Creates an city based in the given object. 
    /// </summary>
    /// <param name="location">CountryDto</param>
    /// <returns>City</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CityFlatData location)
    {
        var query = new CreateCity
        {
            LocationDetails = location
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }


    /// <summary>
    /// Updates an city based in the given object.
    /// </summary>
    /// <param name="location">CountryDto</param>
    /// <returns>Country</returns>
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] CityFlatData location)
    {
        var query = new UpdateCity
        {
            LocationDetails = location
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Does a soft delete on the City with the given id and returns "Success" if no exception was raised.
    /// </summary>
    /// <param name="id">Country Id</param>
    [HttpDelete("Disable/{id}")]
    public async Task<IActionResult> SoftDelete(int id)
    {
        var query = new SoftDeleteCity
        {
            CityId = id
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
        var query = new DeleteCity
        {
            CityId = id
        };
        await _mediator.Send(query);
        return Ok("Success");
    }
}
