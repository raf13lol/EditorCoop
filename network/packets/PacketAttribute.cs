using System;

namespace Network.Packets;

public class PacketAttribute(ushort typeValue, byte version = 0, bool shouldBeReplicated = true) : Attribute
{
    public ushort TypeValue = typeValue;
    public byte Version = version;
    public bool ShouldBeReplicated = shouldBeReplicated;

    public PacketAttribute(object typeValue, byte version = 0, bool shouldBeReplicated = true)
    : this((ushort)typeValue, version, shouldBeReplicated)
    { }

    public PacketAttribute(ushort typeValue, bool shouldBeReplicated)
    : this(typeValue, 0, shouldBeReplicated)
    { }

    public PacketAttribute(object typeValue, bool shouldBeReplicated)
    : this(typeValue, 0, shouldBeReplicated)
    { }
}