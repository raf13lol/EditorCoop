using System;

namespace Network.Packets;

public class PacketMetadata
{
    public Type Type;
    
    public ushort TypeValue;
    public byte Version;
    public bool ShouldBeReplicated;

    public Action<Packet>? HandlerFunction = null;
}