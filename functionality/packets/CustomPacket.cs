using System.IO;
using Network.Packets;

namespace EditorCoop.Functionality.Packets;

public class CustomPacket() : Packet(PacketType.Custom)
{
    public byte[] Data;

    public override void Decode(BinaryReader reader)
    {
        Data = reader.Read<byte[]>();
    }

    public override void Encode(BinaryWriter writer)
    {
        writer.Write<byte[]>(Data);
    }
}