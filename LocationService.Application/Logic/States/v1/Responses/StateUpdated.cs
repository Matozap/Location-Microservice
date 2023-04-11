using LocationService.Message.Contracts.States.v1;
using MediatR;

namespace LocationService.Application.Logic.States.v1.Responses;

public class StateUpdated : IRequest<object>
{
    public StateData Details { get; init; }
}
