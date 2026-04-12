using System;
using System.IO;
using System.Linq;
using System.Reflection;
using EditorCoop;
using EditorCoop.Functionality.Network.Packets;

namespace Network.Packets;

public abstract class Packet(object packetType)
{
    public byte PacketTypeByte = (byte)packetType;

    public abstract void Decode(BinaryReader reader);
    public abstract void Encode(BinaryWriter writer);

    public static Type[] AssemblyTypes;

    public static byte[] Encode(Packet packet)
    {
        using MemoryStream stream = new();
        using BinaryWriter writer = new(stream);

        writer.Write(packet.PacketTypeByte);
        packet.Encode(writer);

        return stream.GetBuffer();
    }

    public static Packet Decode(byte[] data, Type packetTypeEnum)
    {
        using MemoryStream stream = new(data);
        using BinaryReader reader = new(stream);

        byte packetTypeByte = reader.ReadByte();
        string packetType = Enum.GetName(packetTypeEnum, packetTypeByte);
        Type classType = AssemblyTypes.First(t => t.Name == $"{packetType}Packet");

        Packet packet = (Packet)Activator.CreateInstance(classType);
        packet.Decode(reader);
        return packet;
    }
}