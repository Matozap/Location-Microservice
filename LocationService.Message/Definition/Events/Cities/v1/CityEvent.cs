using LocationService.Message.Definition.Protos.Cities.v1;

namespace LocationService.Message.Definition.Events.Cities.v1;

public class CityEvent
{
    public CityData Details { get; init; }
    public EventAction Action { get; init; }
    protected string GetVersion() => "v1";
}

