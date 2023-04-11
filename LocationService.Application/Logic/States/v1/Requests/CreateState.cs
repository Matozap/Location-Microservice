using LocationService.Message.Definition.Protos.States.v1;
using MediatR;

namespace LocationService.Application.Logic.States.v1.Requests;

public class CreateState : IRequest<StateData>
{
    public StateData Details { get; init; }
}