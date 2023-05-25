using System.Runtime.Serialization;

namespace LocationService.Message.Cities;

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