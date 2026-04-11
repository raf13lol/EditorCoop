using System;
using System.IO;
using System.Linq;
using System.Reflection;
using EditorCoop.Functionality.Network.Packets;

namespace Network.Packets;

public abstract class Packet(PacketType type)
{
    public PacketType Type = type;

    public abstract void Decode(BinaryReader reader);
    public abstract void Encode(BinaryWriter writer);

    private static readonly Type[] AssemblyTypes = Assembly.GetAssembly(typeof(Packet)).GetTypes();

    public static Packet Decode(byte[] data, Type packetTypeEnum, Type[]? typesToSearch = null)
    {
        typesToSearch ??= AssemblyTypes;

        using MemoryStream stream = new(data);
        using BinaryReader reader = new(stream);

        object packetType = Convert.ChangeType(reader.ReadByte(), packetTypeEnum);
        Type classType = typesToSearch.First(t => t.Name == $"{packetType}Packet");

        Packet packet = (Packet)Activator.CreateInstance(classType);
        packet.Decode(reader);
        return packet;
    }
}