using System.IO;

namespace Network.Packets;

public abstract class Packet
{
    public abstract void Decode(BinaryReader reader);
    public abstract void Encode(BinaryWriter writer);
}