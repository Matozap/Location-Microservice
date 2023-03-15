using System.Collections.Generic;

namespace LocationService.Domain;

public class Country : EntityBase
{
    public string Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Currency { get; set; }
    public string CurrencyName { get; set; }
    public string Region { get; set; }
    public string SubRegion { get; set; }
    public bool Disabled { get; set; }
    public virtual List<State> States { get; set; }
}