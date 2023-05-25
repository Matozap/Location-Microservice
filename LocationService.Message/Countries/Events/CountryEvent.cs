using LocationService.Message.Common;

namespace LocationService.Message.Countries.Events;

public class CountryEvent
{
    public CountryData Details { get; init; }
    public EventAction Action { get; init; }
}

