using LocationService.Message.Contracts.States.v1;

namespace LocationService.Message.Events.States.v1;

public class StateEvent
{
    public StateData Details { get; init; }
    public EventAction Action { get; init; }
}

