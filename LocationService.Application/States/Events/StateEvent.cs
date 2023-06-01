using LocationService.Application.Common;
using LocationService.Application.States.Responses;

namespace LocationService.Application.States.Events;

public class StateEvent
{
    public StateData Details { get; init; }
    public EventAction Action { get; init; }
}

