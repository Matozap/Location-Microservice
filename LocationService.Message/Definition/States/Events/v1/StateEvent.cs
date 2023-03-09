using LocationService.Message.DataTransfer.States.v1;

namespace LocationService.Message.Definition.States.Events.v1;

public class StateEvent
{
    public StateData LocationDetails { get; init; }
    public EventAction Action { get; set; }
    protected string GetVersion() => "v1";
}

