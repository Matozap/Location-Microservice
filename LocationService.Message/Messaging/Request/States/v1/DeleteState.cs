using MediatR;

namespace LocationService.Message.Messaging.Request.States.v1;

public class DeleteState : BaseMessage, IRequest<object>
{
    public int Id { get; init; }
}
