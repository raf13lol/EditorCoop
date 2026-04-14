using System.IO;
using Network.Packets;

namespace EditorCoop.Functionality.Packets;

public class SyncAssetsPacket() : Packet(PacketType.SyncAssets)
{   
    public bool Finished;
    public byte[] Fragment;

    public override void Decode(BinaryReader reader)
    {
    }

    public override void Encode(BinaryWriter writer)
    {
    }
}