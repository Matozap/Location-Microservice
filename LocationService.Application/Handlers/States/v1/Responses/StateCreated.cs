using LocationService.Message.Contracts.States.v1;
using MediatR;

namespace LocationService.Application.Handlers.States.v1.Responses;

public class StateCreated : IRequest<object>
{
    public StateData Details { get; init; }
}
