using LocationService.Message.DTO.States.v1;
using MediatR;

namespace LocationService.Message.Messaging.Request.States.v1;

public class UpdateState : BaseMessage, IRequest<object>
{
    public StateData LocationDetails { get; init; }
}
