using LocationService.Message.DataTransfer.Cities.v1;
using MediatR;

namespace LocationService.Message.Definition.Cities.Responses.v1;

public class CityCreated : IRequest<object>
{
    public CityData Details { get; init; }
}
