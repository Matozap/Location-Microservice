using LocationService.Message.DTO.Countries.v1;

namespace LocationService.Message.Messaging.Event.v1;

public class CountryEvent : BaseEvent
{
    public CountryFlatData LocationDetails { get; init; }
    protected override string GetVersion() => "V1";
}

