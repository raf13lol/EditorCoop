using System.IO;
using Network.Packets;

namespace EditorCoop.Functionality.Packets.Assets;

[Packet(PacketType.SyncAssets)]
public class SyncAssetsPacket() : Packet()
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