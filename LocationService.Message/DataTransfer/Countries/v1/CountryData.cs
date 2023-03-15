using System.Collections.Generic;
using LocationService.Message.DataTransfer.States.v1;

namespace LocationService.Message.DataTransfer.Countries.v1;

public class CountryData
{
    public string Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Currency { get; set; }
    public string CurrencyName { get; set; }
    public string Region { get; set; }
    public string SubRegion { get; set; }
    
    public List<StateData> States { get; set; }
}