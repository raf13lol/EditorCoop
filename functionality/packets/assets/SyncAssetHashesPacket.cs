using System.Collections.Generic;
using System.IO;
using EditorCoop.Functionality.Data.Assets;
using Network.Packets;

namespace EditorCoop.Functionality.Packets.Assets;

[Packet(PacketType.SyncAssetHashes)]
public class SyncAssetHashesPacket() : Packet()
{
    public List<AssetHashData> Hashes;

    public override void Decode(BinaryReader reader)
    {
        Hashes = reader.Read<List<AssetHashData>>();
    }

    public override void Encode(BinaryWriter writer)
    {
        writer.Write(Hashes);
    }
}