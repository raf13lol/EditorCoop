using System.IO;
using Network.Packets;

namespace EditorCoop.Functionality.Packets.Assets;

[Packet(PacketType.SyncAssets)]
public class SyncAssetsPacket() : Packet()
{
    public int UID;
    public bool Finished;
    public byte[] Fragment;

    public override void Decode(BinaryReader reader)
    {
        UID = reader.ReadInt32();
        Finished = reader.ReadBoolean();
        int length = reader.ReadInt32();
        Fragment = reader.ReadBytes(length);
    }

    public override void Encode(BinaryWriter writer)
    {
        writer.Write(UID);
        writer.Write(Finished);
        writer.Write(Fragment.Length);
        writer.Write(Fragment);
    }
}