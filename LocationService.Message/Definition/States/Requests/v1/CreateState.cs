using LocationService.Message.DataTransfer.States.v1;
using MediatR;

namespace LocationService.Message.Definition.States.Requests.v1;

public class CreateState : BaseMessage, IRequest<object>
{
    public StateData Details { get; init; }
}