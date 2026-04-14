using System.IO;
using Network.Packets;

namespace EditorCoop.Functionality.Data.Events;

public class EventMovementData() : IPacketData
{
    public int UID;

    public int Bar;
    public float Beat;

    public int Y;

    public int Row;
    public string? Target;

    public void Decode(BinaryReader reader)
    {
        UID = reader.ReadInt32();

        Bar = reader.ReadInt32();
        Beat = reader.ReadSingle();

        Y = reader.ReadInt32();

        Row = reader.ReadInt32();
        Target = reader.Read<string>();
    }

    public void Encode(BinaryWriter writer)
    {
        writer.Write(UID);

        writer.Write(Bar);
        writer.Write(Beat);
        
        writer.Write(Y);

        writer.Write(Row);
        writer.Write<string>(Target);
    }
}