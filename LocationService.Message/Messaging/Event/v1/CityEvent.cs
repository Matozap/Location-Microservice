using LocationService.Message.DTO.Cities.v1;

namespace LocationService.Message.Messaging.Event.v1;

public class CityEvent
{
    public CityData LocationDetails { get; init; }
    public EventAction Action { get; set; }
    protected string GetVersion() => "v1";
}

