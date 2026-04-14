using System.IO;
using Network.Packets;

namespace EditorCoop.Functionality.Packets;

[Packet(PacketType.__Template)]
public class TemplatePacket() : Packet()
{
    public override void Decode(BinaryReader reader)
    {
    }

    public override void Encode(BinaryWriter writer)
    {
    }
}