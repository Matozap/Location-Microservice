using System.Collections.Generic;

namespace LocationService.Infrastructure.Data.Queue;

public class EventBusOptions
{
    public string Destination { get; set; }
    public string ConnectionString { get; set; }
    public bool UseInMemory { get; set; }
    public bool Disabled { get; set; }
    public Dictionary<string, string> SendToAdditionalPath { get; set; }
    public Dictionary<string, string> Subscriptions { get; set; }
}
