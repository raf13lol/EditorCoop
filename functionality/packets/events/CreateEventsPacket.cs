using System.Collections.Generic;
using System.IO;
using EditorCoop.Functionality.Data.Events;
using Network.Packets;

namespace EditorCoop.Functionality.Packets.Events;

[Packet(PacketType.CreateEvents)]
public class CreateEventsPacket() : Packet()
{
    public List<EventCreationData> Events;

    public override void Decode(BinaryReader reader)
    {
        Events = reader.Read<List<EventCreationData>>();
    }

    public override void Encode(BinaryWriter writer)
    {
        writer.Write(Events);
    }
}