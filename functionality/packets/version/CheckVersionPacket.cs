using System.IO;
using Network.Packets;

namespace EditorCoop.Functionality.Packets.Version;

[Packet(PacketType.CheckVersion)]
public class CheckVersionPacket() : Packet()
{
    // Made for compat reasons with Multiplayer mod, but i need to make it compatible with this....
    public bool IsEditorLobby;
    public string Commit;

    public override void Decode(BinaryReader reader)
    {
        IsEditorLobby = reader.ReadBoolean();
        if (!IsEditorLobby)
            return;
        Commit = reader.ReadString();
    }

    public override void Encode(BinaryWriter writer)
    {
        writer.Write(IsEditorLobby);
        writer.Write(Commit);
    }
}