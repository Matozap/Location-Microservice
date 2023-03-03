using LocationService.Message.DTO.Cities.v1;
using MediatR;

namespace LocationService.Message.Messaging.Request.Cities.v1;

public class UpdateCity : BaseMessage, IRequest<object>
{
    public CityData LocationDetails { get; init; }
}
