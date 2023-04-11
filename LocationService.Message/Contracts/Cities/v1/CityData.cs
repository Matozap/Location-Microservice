using ProtoBuf;

namespace LocationService.Message.Contracts.Cities.v1;

[ProtoContract]
public class CityData
{
    [ProtoMember(1, DataFormat = DataFormat.WellKnown)]
    public string Id { get; set; }
    [ProtoMember(2, DataFormat = DataFormat.WellKnown)]
    public string Name { get; set; }
    [ProtoMember(3, DataFormat = DataFormat.WellKnown)]
    public string StateId { get; set; }
}