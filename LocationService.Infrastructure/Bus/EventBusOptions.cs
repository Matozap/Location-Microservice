using System.Collections.Generic;

namespace LocationService.Infrastructure.Bus;

public class EventBusOptions
{
    public string ConnectionString { get; set; }
    public string BusType { get; set; }
    public bool Disabled { get; set; }
    public Dictionary<string, string> SendToAdditionalPath { get; set; }
    public Dictionary<string, string> Subscriptions { get; set; }
}
