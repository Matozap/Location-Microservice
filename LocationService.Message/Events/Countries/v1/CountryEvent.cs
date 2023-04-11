using LocationService.Message.Contracts.Countries.v1;

namespace LocationService.Message.Events.Countries.v1;

public class CountryEvent
{
    public CountryData Details { get; init; }
    public EventAction Action { get; init; }
}

