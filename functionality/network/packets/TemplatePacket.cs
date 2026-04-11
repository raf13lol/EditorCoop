using System.IO;
using Network.Packets;

namespace EditorCoop.Functionality.Network.Packets;

public class TemplatePacket() : Packet(PacketType.__Template)
{
    public override void Decode(BinaryReader reader)
    {
    }

    public override void Encode(BinaryWriter writer)
    {
    }
}