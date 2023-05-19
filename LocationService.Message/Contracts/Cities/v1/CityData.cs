using System.Runtime.Serialization;

namespace LocationService.Message.Contracts.Cities.v1;

[DataContract]
public class CityData
{
    [DataMember(Order = 1)]
    public string Id { get; set; }
    [DataMember(Order = 2)]
    public string Name { get; set; }
    [DataMember(Order = 3)]
    public string StateId { get; set; }
}