using System.IO;

namespace Network.Packets.Data;

public interface IPacketData
{
    public void Decode(BinaryReader reader);
    public void Encode(BinaryWriter writer);
}