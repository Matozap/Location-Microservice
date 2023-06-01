using LocationService.Application.Cities.Responses;
using LocationService.Application.Common;

namespace LocationService.Application.Cities.Events;

public class CityEvent
{
    public CityData Details { get; init; }
    public EventAction Action { get; init; }
}

