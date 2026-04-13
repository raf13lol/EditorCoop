using System.Collections.Generic;
using System.IO;
using Network.Packets;

namespace EditorCoop.Functionality.Data.Events;

public class BasePropertiesEditData() : IPacketData
{
    public List<int> Conditionals = null;
    public List<string> GlobalConditionals = null;
    public int[] Rooms = null;
    public string Tag = null;
    public bool RunTagNormally; // No need to send an extra byte....

    public void Decode(BinaryReader reader)
    {
        Conditionals = reader.Read<List<int>>();
        GlobalConditionals = reader.Read<List<string>>();
        Rooms = reader.Read<int[]>();
        Tag = reader.Read<string>();
        RunTagNormally = reader.Read<bool>();
    }

    public void Encode(BinaryWriter writer)
    {
        writer.Write(Conditionals);
        writer.Write(GlobalConditionals);
        writer.Write(Rooms);
        writer.Write<string>(Tag);
        writer.Write(RunTagNormally);
    }
}
