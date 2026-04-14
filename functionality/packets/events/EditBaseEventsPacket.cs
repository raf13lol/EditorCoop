using System.Collections.Generic;
using System.IO;
using EditorCoop.Functionality.Data.Events;
using Network.Packets;

namespace EditorCoop.Functionality.Packets.Events;

[Packet(PacketType.EditBaseEvents)]
public class EditBaseEventsPacket() : Packet()
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