using LocationService.Application.Interfaces;
using LocationService.Message.Contracts.Cities.v1;

namespace LocationService.Application.Handlers.Cities.v1.Requests;

public class GetCityById : IQuery<CityData>
{
    public string Id { get; init; }
}
