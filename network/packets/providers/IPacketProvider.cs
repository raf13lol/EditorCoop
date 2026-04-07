using System;
using System.IO;

namespace Network.Packets.Providers;

public interface IPacketProvider
{
    public object Read(BinaryReader reader, Type type, out bool success);
    public void Write(BinaryWriter writer, Type type, object value, out bool success);
}