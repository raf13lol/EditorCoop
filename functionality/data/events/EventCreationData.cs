using System.IO;
using Network.Packets;
using RDLevelEditor;

namespace EditorCoop.Functionality.Data.Events;

public class EventCreationData() : IPacketData
{
    public int UID;
    public LevelEventType EventType;

    public int Bar;
    public float Beat;

    public Tab Tab;
    public int Y;

    public int Row;
    public string Target;

    public int? CopyEventUID;

    public void Decode(BinaryReader reader)
    {
        UID = reader.ReadInt32();
        EventType = reader.Read<LevelEventType>();

        Bar = reader.ReadInt32();
        Beat = reader.ReadSingle();

        Tab = reader.Read<Tab>();
        Y = reader.ReadInt32();

        Row = reader.ReadInt32();
        Target = reader.Read<string>();

        CopyEventUID = reader.Read<int?>();
    }

    public void Encode(BinaryWriter writer)
    {
        writer.Write(UID);
        writer.Write(EventType);

        writer.Write(Bar);
        writer.Write(Beat);
        
        writer.Write(Tab);
        writer.Write(Y);

        writer.Write(Row);
        writer.Write<string>(Target);

        writer.Write(CopyEventUID);
    }
}