using System.Collections.Generic;

namespace LocationService.Domain;

public class State : EntityBase
{
    public int Id { get; set; }
    
    public string Code { get; set; }
    public string Name { get; set; }
    public bool Disabled { get; set; }
    public string CountryId { get; set; }
    public virtual Country Country { get; set; }
    public virtual List<City> Cities { get; set; }
}