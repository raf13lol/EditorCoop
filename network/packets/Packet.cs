using System;
using System.IO;
using Network.Packets.Types;

namespace Network.Packets;

public abstract class Packet
{
    public abstract void Decode(BinaryReader reader);
    public abstract void Encode(BinaryWriter writer);

    public static Packet Decode(byte[] data)
    {
        using MemoryStream stream = new(data);
        using BinaryReader reader = new(stream);

        PacketType packetType = (PacketType)reader.ReadByte();
        Type classType = Type.GetType($"{packetType}Packet");
        
        Packet packet = (Packet)Activator.CreateInstance(classType); 
        packet.Decode(reader);
        return packet;
    }
}