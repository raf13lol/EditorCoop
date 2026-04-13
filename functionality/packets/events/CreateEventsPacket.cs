using System.Collections.Generic;
using System.IO;
using EditorCoop.Functionality.Data.Events;
using Network.Packets;

namespace EditorCoop.Functionality.Packets.Events;

public class CreateEventsPacket() : Packet(PacketType.CreateEvents)
{
    public List<EventCreationData> Events;

    public override void Decode(BinaryReader reader)
    {
        Events = PacketBinary.Read<List<EventCreationData>>(reader);
    }

    public override void Encode(BinaryWriter writer)
    {
        PacketBinary.Write(writer, Events);
    }
}