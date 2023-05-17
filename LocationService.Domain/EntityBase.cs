using System;

namespace LocationService.Domain;

public class EntityBase
{
    public string Id { get; set; }
    public DateTime LastUpdateDate { get; set; } = DateTime.UtcNow;

    public string LastUpdateUserId { get; set; } = "System";
}
