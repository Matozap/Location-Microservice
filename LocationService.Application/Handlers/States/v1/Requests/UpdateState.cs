using LocationService.Message.Contracts.States.v1;
using MediatR;

namespace LocationService.Application.Handlers.States.v1.Requests;

public class UpdateState : IRequest<StateData>
{
    public StateData Details { get; init; }
}
