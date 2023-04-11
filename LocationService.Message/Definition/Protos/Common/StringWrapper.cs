using System.Diagnostics.CodeAnalysis;
using ProtoBuf;

namespace LocationService.Message.Definition.Protos.Common;

[ExcludeFromCodeCoverage]
[ProtoContract]
public class StringWrapper
{
    [ProtoMember(1, DataFormat = DataFormat.WellKnown)]
    public string Value { get; set; }
}