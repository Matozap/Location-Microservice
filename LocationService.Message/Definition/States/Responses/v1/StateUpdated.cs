using LocationService.Message.DataTransfer.States.v1;
using MediatR;

namespace LocationService.Message.Definition.States.Responses.v1;

public class StateUpdated : IRequest<object>
{
    public StateData Details { get; init; }
}
