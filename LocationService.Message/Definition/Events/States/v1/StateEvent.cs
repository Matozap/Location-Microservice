using LocationService.Message.Definition.Protos.States.v1;

namespace LocationService.Message.Definition.Events.States.v1;

public class StateEvent
{
    public StateData Details { get; init; }
    public EventAction Action { get; init; }
    protected string GetVersion() => "v1";
}

