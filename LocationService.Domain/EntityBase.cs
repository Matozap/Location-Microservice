using System;

namespace LocationService.Domain;

public class EntityBase
{
    public string Id { get; set; }
    public DateTime LastUpdateDate { get; set; }

    public string LastUpdateUserId { get; set; }
}
