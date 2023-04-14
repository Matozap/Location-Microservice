using LocationService.Message.Contracts.Cities.v1;
using MediatR;

namespace LocationService.Application.Handlers.Cities.v1.Requests;

public class GetCityById : IRequest<CityData>
{
    public string Id { get; init; }
}
