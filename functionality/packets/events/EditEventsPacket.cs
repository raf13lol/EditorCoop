using System.Collections.Generic;
using System.IO;
using EditorCoop.Functionality.Data.Events;
using Network.Packets;

namespace EditorCoop.Functionality.Packets.Events;

public class EditEventsPacket() : Packet(PacketType.EditEvents)
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