using System.IO;

namespace Network.Packets.Types;

public class CheckVersionPacket : Packet
{
    // Made for compat reasons with Multiplayer mod, but i need to make it compatible with this....
    public bool IsEditorLobby;
    public string Commit;
    public string PluginVersion;

    public override void Decode(BinaryReader reader)
    {
        IsEditorLobby = reader.ReadBoolean();
        Commit = reader.ReadString();
        PluginVersion = reader.ReadString();
    }

    public override void Encode(BinaryWriter writer)
    {
        writer.Write(IsEditorLobby);
        writer.Write(Commit);
        writer.Write(PluginVersion);
    }
}