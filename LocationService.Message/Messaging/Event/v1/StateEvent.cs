using LocationService.Message.DTO.States.v1;

namespace LocationService.Message.Messaging.Event.v1;

public class StateEvent
{
    public StateData LocationDetails { get; init; }
    public EventAction Action { get; set; }
    protected string GetVersion() => "v1";
}

