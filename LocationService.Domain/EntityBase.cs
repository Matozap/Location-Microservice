using System;

namespace LocationService.Domain;

public class EntityBase
{
    public DateTime LastUpdateDate { get; set; }

    public string LastUpdateUserId { get; set; }
}
