using System.IO;

namespace Network.Packets;

public class UnknownPacket() : Packet()
{
    public byte[] Data;

    public override void Decode(BinaryReader reader)
    {
        Data = reader.ReadBytes((int)(reader.BaseStream.Length - reader.BaseStream.Position));
    }

    public override void Encode(BinaryWriter writer)
    {
        writer.Write(Data);
    }
}