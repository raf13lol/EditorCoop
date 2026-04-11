using System.IO;
using Network.Packets;

namespace EditorCoop.Functionality.Network.Packets;

public class CheckVersionPacket() : Packet(PacketType.CheckVersion)
{
    // Made for compat reasons with Multiplayer mod, but i need to make it compatible with this....
    public bool IsEditorLobby;
    public string Commit;
    public string PluginVersion;

    public override void Decode(BinaryReader reader)
    {
        IsEditorLobby = reader.ReadBoolean();
        if (!IsEditorLobby)
            return;
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