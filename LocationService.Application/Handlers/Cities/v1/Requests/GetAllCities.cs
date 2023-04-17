using System.Collections.Generic;
using LocationService.Application.Interfaces;
using LocationService.Message.Contracts.Cities.v1;

namespace LocationService.Application.Handlers.Cities.v1.Requests;

public class GetAllCities : IQuery<List<CityData>>
{
    public string StateId { get; init; }
}
