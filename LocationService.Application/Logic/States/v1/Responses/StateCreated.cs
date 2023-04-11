using LocationService.Message.Definition.Protos.States.v1;
using MediatR;

namespace LocationService.Application.Logic.States.v1.Responses;

public class StateCreated : IRequest<object>
{
    public StateData Details { get; init; }
}
