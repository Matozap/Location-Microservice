using LocationService.Message.DataTransfer.Cities.v1;
using MediatR;

namespace LocationService.Message.Definition.Cities.Requests.v1;

public class CreateCity : BaseMessage, IRequest<object>
{
    public CityData LocationDetails { get; init; }
}