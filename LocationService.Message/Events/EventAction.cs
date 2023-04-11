namespace LocationService.Message.Events;

public enum EventAction
{
    None,
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
