using LocationService.Message.Common;

namespace LocationService.Message.States.Events;

public class StateEvent
{
    public StateData Details { get; init; }
    public EventAction Action { get; init; }
}

