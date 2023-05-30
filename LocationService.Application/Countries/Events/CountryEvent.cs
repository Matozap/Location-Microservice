using LocationService.Application.Common;
using LocationService.Application.Countries.Responses;

namespace LocationService.Application.Countries.Events;

public class CountryEvent
{
    public CountryData Details { get; init; }
    public EventAction Action { get; init; }
}

