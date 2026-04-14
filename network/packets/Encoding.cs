using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Network.Packets;

public class Encoding
{
    public static bool CallingHandler = false;

    public const ushort UnknownPacketTypeValue = 0xffff;

    public static Dictionary<ushort, PacketMetadata> Metadata = new()
    {
        [UnknownPacketTypeValue] = new()
        {
            TypeValue = UnknownPacketTypeValue,
            Version = 0x00,
            ShouldBeReplicated = false,

            Type = typeof(UnknownPacket)
        }
    };

    public static Dictionary<Type, ushort> PacketTypeLookup = new()
    {
        [typeof(UnknownPacket)] = UnknownPacketTypeValue
    };

    public static void Register(Type packetType, params PacketMetadata.PacketHandler[] handlerFunctions)
    {
        PacketAttribute attribute = packetType.GetCustomAttribute<PacketAttribute>();
        if (attribute == null)
            throw new ArgumentException($"{packetType.Name} does not have a {nameof(PacketAttribute)} attribute.");

        PacketMetadata metadata = new()
        {
            Type = packetType,

            TypeValue = attribute.TypeValue,
            Version = attribute.Version,
            ShouldBeReplicated = attribute.ShouldBeReplicated
        };

        foreach (PacketMetadata.PacketHandler handlerFunction in handlerFunctions)
            metadata.HandlerFunction += handlerFunction;

        Register(metadata);
    }

    public static void Register(PacketMetadata metadata)
    {
        Metadata.Add(metadata.TypeValue, metadata);
        PacketTypeLookup.Add(metadata.Type, metadata.TypeValue);
    }

    public static byte[] Encode(Packet packet)
    {
        using MemoryStream stream = new();
        using BinaryWriter writer = new(stream);

        writer.Write(packet.TypeValue);
        writer.Write(packet.Version);
        writer.Write(packet.ShouldBeReplicated);
        packet.Encode(writer);

        return stream.GetBuffer();
    }

    public static Packet Decode(byte[] data)
    {
        using MemoryStream stream = new(data);
        using BinaryReader reader = new(stream);

        ushort packetTypeValue = reader.ReadUInt16();
        byte version = reader.ReadByte();
        bool shouldBeReplicated = reader.ReadBoolean();

        if (!Metadata.TryGetValue(packetTypeValue, out PacketMetadata metadata) || metadata.Version != version)
            metadata = Metadata[UnknownPacketTypeValue];

        Packet packet = (Packet)Activator.CreateInstance(metadata.Type);
        packet.Decode(reader);

        packet.TypeValue = packetTypeValue;
        packet.Version = version;
        packet.ShouldBeReplicated = shouldBeReplicated;

        CallingHandler = true;
        metadata.CallHandler(packet);
        CallingHandler = false;

        return packet;
    }
}