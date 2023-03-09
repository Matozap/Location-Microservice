using System.Collections.Generic;
using LocationService.Message.DataTransfer.States.v1;

namespace LocationService.Message.DataTransfer.Countries.v1;

public class CountryData
{
    public string Id { get; set; }
    public string Name { get; set; }
    
    public List<StateData> States { get; set; }
}