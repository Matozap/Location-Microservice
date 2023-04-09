using LocationService.Message.DataTransfer.Cities.v1;
using MediatR;

namespace LocationService.Message.Definition.Cities.Requests.v1;

public class GetCityById : BaseMessage, IRequest<CityData>
{
    public string Id { get; init; }
}
