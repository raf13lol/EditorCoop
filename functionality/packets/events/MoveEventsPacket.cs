using System.Collections.Generic;
using System.IO;
using EditorCoop.Functionality.Data.Events;
using Network.Packets;

namespace EditorCoop.Functionality.Packets.Events;

public class MoveEventsPacket() : Packet(PacketType.MoveEvents)
{
    public List<EventMovementData> Movements;

    public override void Decode(BinaryReader reader)
    {
        Movements = reader.Read<List<EventMovementData>>();
    }

    public override void Encode(BinaryWriter writer)
    {
        writer.Write(Movements);
    }
}