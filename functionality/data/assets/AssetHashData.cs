using System.IO;
using Network.Packets;

namespace EditorCoop.Functionality.Data.Assets;

public class AssetHashData() : IPacketData
{
    public string Filename;
    public byte[] Hash;

    public void Decode(BinaryReader reader)
    {
        Filename = reader.ReadString();
        Hash = reader.Read<byte[]>();
    }

    public void Encode(BinaryWriter writer)
    {
        writer.Write(Filename);
        writer.Write<byte[]>(Hash);
    }
}