using LocationService.Message.Contracts.Cities.v1;

namespace LocationService.Message.Events.Cities.v1;

public class CityEvent
{
    public CityData Details { get; init; }
    public EventAction Action { get; init; }
}

