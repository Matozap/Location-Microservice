using System.Collections.Generic;

namespace LocationService.Domain;

public class Country : EntityBase
{
    public string Id { get; set; }
    public string Name { get; set; }
    public bool Disabled { get; set; }
    public virtual List<State> States { get; set; }
}