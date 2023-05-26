using System.Collections.Generic;
using System.Runtime.Serialization;
using LocationService.Message.Cities;

namespace LocationService.Message.States;

[DataContract]
public class StateData
{
    [DataMember(Order = 1)]
    public string Id { get; set; }
    [DataMember(Order = 2)]
    public string Code { get; init; }
    [DataMember(Order = 3)]
    public string Name { get; set; }
    [DataMember(Order = 4)]
    public string CountryId { get; set; }
    [DataMember(Order = 5)]
    public IReadOnlyCollection<CityData> Cities { get; set; }
}