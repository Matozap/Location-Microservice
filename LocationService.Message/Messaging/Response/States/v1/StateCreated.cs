using LocationService.Message.DTO.States.v1;
using MediatR;

namespace LocationService.Message.Messaging.Response.States.v1;

public class StateCreated : IRequest<object>
{
    public StateData LocationDetails { get; init; }
}
