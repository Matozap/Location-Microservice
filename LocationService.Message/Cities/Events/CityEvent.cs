using LocationService.Message.Common;

namespace LocationService.Message.Cities.Events;

public class CityEvent
{
    public CityData Details { get; init; }
    public EventAction Action { get; init; }
}

