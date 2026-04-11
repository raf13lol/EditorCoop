using System.IO;
using Network.Packets;

namespace EditorCoop.Functionality.Network.Packets;

// Empty as this packet exists only to keep connection alive
public class PingPacket() : Packet
{
    public override void Decode(BinaryReader reader)
    {
    }

    public override void Encode(BinaryWriter writer)
    {
    }
}