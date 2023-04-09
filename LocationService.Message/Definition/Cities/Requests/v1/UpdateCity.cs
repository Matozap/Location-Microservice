using LocationService.Message.DataTransfer.Cities.v1;
using MediatR;

namespace LocationService.Message.Definition.Cities.Requests.v1;

public class UpdateCity : BaseMessage, IRequest<CityData>
{
    public CityData Details { get; init; }
}
