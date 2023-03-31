using LocationService.Message.DataTransfer.Countries.v1;

namespace LocationService.Message.Definition.Countries.Events.v1;

public class CountryEvent
{
    public CountryData Details { get; init; }
    public EventAction Action { get; init; }
    protected string GetVersion() => "v1";
}

