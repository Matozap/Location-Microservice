using LocationService.Message.DTO.Countries.v1;

namespace LocationService.Message.Messaging.Event.v1;

public class CountryEvent
{
    public CountryData LocationDetails { get; init; }
    public EventAction Action { get; set; }
    protected string GetVersion() => "v1";
}

