using MediatR;

namespace LocationService.Message.Messaging.Request.Cities.v1;

public class GetAllCities : BaseMessage, IRequest<object>
{
    public int StateId { get; set; }
}
