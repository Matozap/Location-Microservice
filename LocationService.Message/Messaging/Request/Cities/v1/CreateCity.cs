using LocationService.Message.DTO.Cities.v1;
using MediatR;

namespace LocationService.Message.Messaging.Request.v1;

public class CreateCity : BaseMessage, IRequest<object>
{
    public CityFlatData LocationDetails { get; init; }
}