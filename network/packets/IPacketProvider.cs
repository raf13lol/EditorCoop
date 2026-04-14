using System;
using System.IO;

namespace Network.Packets;

public interface IPacketProvider
{
    public bool Read(BinaryReader reader, Type type, out object value);
    public bool Write(BinaryWriter writer, Type type, object value);
}