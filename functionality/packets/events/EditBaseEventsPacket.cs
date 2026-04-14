using System.Collections.Generic;
using System.IO;
using EditorCoop.Functionality.Data.Events;
using Network.Packets;

namespace EditorCoop.Functionality.Packets.Events;

public class EditBaseEventsPacket() : Packet(PacketType.EditBaseEvents)
{
    public List<BaseEventEditData> Edits;

    public override void Decode(BinaryReader reader)
    {
        Edits = reader.Read<List<BaseEventEditData>>();
    }

    public override void Encode(BinaryWriter writer)
    {
        writer.Write(Edits);
    }
}