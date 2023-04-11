using LocationService.Domain;
using LocationService.Message.Definition.Events;

namespace LocationService.Infrastructure.Bus;

public class EventBusOutbox : EntityBase
{
    public string JsonObject { get; init; }
    public EventAction Action { get; set; }
}