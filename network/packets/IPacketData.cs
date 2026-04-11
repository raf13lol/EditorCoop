using System.IO;

namespace Network.Packets;

public interface IPacketData
{
    public void Decode(BinaryReader reader);
    public void Encode(BinaryWriter writer);
}