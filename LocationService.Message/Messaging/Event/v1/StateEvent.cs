using LocationService.Message.DTO.States.v1;

namespace LocationService.Message.Messaging.Event.v1;

public class StateEvent : BaseEvent
{
    public StateData LocationDetails { get; init; }
    protected override string GetVersion() => "V1";
}

