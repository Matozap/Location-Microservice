using MediatR;

namespace LocationService.Message.Messaging.Response.States.v1;

public class StateDeleted : IRequest<object>
{
    public string StateId { get; init; }
}
