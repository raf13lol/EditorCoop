using System.IO;
using Network.Packets;
using RDLevelEditor;

namespace EditorCoop.Functionality.Data.Events;

public class EventCreationData() : IPacketData
{
    public int EventUID;
    public LevelEventType EventType;

    public int Bar;
    public float Beat;

    public Tab Tab;
    public int Y;
    public int SubRowY;

    public int? Row;
    public string Target;

    public int? CopyEventUID;

    public void Decode(BinaryReader reader)
    {
        EventUID = reader.ReadInt32();
        EventType = PacketBinary.Read<LevelEventType>(reader);

        Bar = reader.ReadInt32();
        Beat = reader.ReadSingle();

        Tab = PacketBinary.Read<Tab>(reader);
        Y = reader.ReadInt32();
        SubRowY = reader.ReadInt32();

        Row = PacketBinary.Read<int?>(reader);
        Target = reader.ReadString();

        CopyEventUID = PacketBinary.Read<int?>(reader);
    }

    public void Encode(BinaryWriter writer)
    {
        writer.Write(EventUID);
        PacketBinary.Write(writer, EventType);

        writer.Write(Bar);
        writer.Write(Beat);
        
        PacketBinary.Write(writer, Tab);
        writer.Write(Y);
        writer.Write(SubRowY);

        PacketBinary.Write(writer, Row);
        writer.Write(Target);

        PacketBinary.Write(writer, CopyEventUID);
    }
}