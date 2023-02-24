using LocationService.Message.DTO.Cities.v1;

namespace LocationService.Message.Messaging.Event.v1;

public class CityEvent : BaseEvent
{
    public CityFlatData LocationDetails { get; init; }
    protected override string GetVersion() => "V1";
}

