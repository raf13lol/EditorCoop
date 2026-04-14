using System;

namespace Network.Packets;

public class PacketMetadata
{
    public Type Type;
    
    public ushort TypeValue;
    public byte Version;
    public bool ShouldBeReplicated;

    public delegate void PacketHandler(Packet packet);
    public event PacketHandler HandlerFunction;

    internal void CallHandler(Packet packet)
    {
        HandlerFunction(packet);
    }
}