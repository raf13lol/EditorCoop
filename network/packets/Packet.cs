using System.IO;

namespace Network.Packets;

public abstract class Packet
{
    public PacketMetadata GeneralMetadata
    {
        get
        {
            if (Encoding.Metadata.TryGetValue(TypeValue, out PacketMetadata metadata))
                return metadata;
            return Encoding.Metadata[Encoding.UnknownPacketTypeValue];
        }
    }

    public ushort TypeValue;
    public byte Version;
    public bool ShouldBeReplicated;

    public Packet()
    {
        TypeValue = Encoding.PacketTypeLookup[GetType()];
        Version = GeneralMetadata.Version;
        ShouldBeReplicated = GeneralMetadata.ShouldBeReplicated;
    }

    public abstract void Decode(BinaryReader reader);
    public abstract void Encode(BinaryWriter writer);
}