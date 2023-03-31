using LocationService.Message.DataTransfer.States.v1;

namespace LocationService.Message.Definition.States.Events.v1;

public class StateEvent
{
    public StateData Details { get; init; }
    public EventAction Action { get; init; }
    protected string GetVersion() => "v1";
}

