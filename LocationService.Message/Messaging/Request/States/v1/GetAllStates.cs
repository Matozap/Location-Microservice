using MediatR;

namespace LocationService.Message.Messaging.Request.States.v1;

public class GetAllStates : BaseMessage, IRequest<object>
{
    public string CountryId { get; set; }
}
