namespace LocationService.Message.Messaging.Event;

public class BaseEvent
{
    public EventAction Action { get; set; }
    protected virtual string GetVersion() => "V1";
}

public enum EventAction
{
    LocationCreate,
    LocationUpdate,
    LocationDelete,
    CountryCreate,
    CountryUpdate,
    CountryDelete,
    StateCreate,
    StateUpdate,
    StateDelete,
    CityCreate,
    CityUpdate,
    CityDelete
}
