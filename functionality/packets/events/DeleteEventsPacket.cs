using System.Collections.Generic;
using System.IO;
using Network.Packets;

namespace EditorCoop.Functionality.Packets.Events;

public class DeleteEventsPacket() : Packet(PacketType.DeleteEvents)
{
    public List<int> UIDs;

    public override void Decode(BinaryReader reader)
    {
        UIDs = reader.Read<List<int>>();
    }

    public override void Encode(BinaryWriter writer)
    {
        writer.Write(UIDs);
    }
}