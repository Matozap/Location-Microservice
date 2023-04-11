using LocationService.Message.Definition.Protos.Countries.v1;

namespace LocationService.Message.Definition.Events.Countries.v1;

public class CountryEvent
{
    public CountryData Details { get; init; }
    public EventAction Action { get; init; }
}

