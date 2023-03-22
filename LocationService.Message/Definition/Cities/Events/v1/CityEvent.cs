using LocationService.Message.DataTransfer.Cities.v1;

namespace LocationService.Message.Definition.Cities.Events.v1;

public class CityEvent
{
    public CityData LocationDetails { get; init; }
    public EventAction Action { get; init; }
    protected string GetVersion() => "v1";
}

