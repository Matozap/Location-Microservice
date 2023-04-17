using LocationService.Application.Interfaces;
using LocationService.Message.Contracts.States.v1;

namespace LocationService.Application.Handlers.States.v1.Requests;

public class UpdateState : ICommand<StateData>
{
    public StateData Details { get; init; }
}
