using System.Collections.Generic;
using LocationService.Message.DTO.Cities.v1;

namespace LocationService.Message.DTO.States.v1;

public class StateData
{
    public int Id { get; set; }
    public string Code { get; init; }
    public string Name { get; set; }
    public string CountryId { get; set; }
    public virtual List<CityData> Cities { get; set; }
}