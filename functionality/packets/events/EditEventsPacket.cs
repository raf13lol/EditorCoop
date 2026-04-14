using System.Collections.Generic;
using System.IO;
using EditorCoop.Functionality.Data.Events;
using Network.Packets;

namespace EditorCoop.Functionality.Packets.Events;

[Packet(PacketType.EditEvents)]
public class EditEventsPacket() : Packet()
{
    public List<EventEditData> Edits;

    public override void Decode(BinaryReader reader)
    {
        Edits = reader.Read<List<EventEditData>>();
    }

    public override void Encode(BinaryWriter writer)
    {
        writer.Write(Edits);
    }
}