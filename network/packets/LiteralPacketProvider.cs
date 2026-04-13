using System;
using System.IO;

namespace Network.Packets;

public class LiteralPacketProvider : IPacketProvider
{
    public object Read(BinaryReader reader, Type type, out bool success)
    {
        success = true;

        if (type == typeof(sbyte)) return reader.ReadSByte();
        if (type == typeof(byte)) return reader.ReadByte();
        if (type == typeof(bool)) return reader.ReadBoolean();

        if (type == typeof(short)) return reader.ReadInt16();
        if (type == typeof(ushort)) return reader.ReadUInt16();

        if (type == typeof(int)) return reader.ReadInt32();
        if (type == typeof(uint)) return reader.ReadUInt32();

        if (type == typeof(long)) return reader.ReadUInt64();
        if (type == typeof(ulong)) return reader.ReadUInt64();

        if (type == typeof(float)) return reader.ReadSingle();
        if (type == typeof(double)) return reader.ReadDouble();
        if (type == typeof(decimal)) return reader.ReadDecimal();

        if (type == typeof(char)) return reader.ReadChar();
        if (type == typeof(string))
        {
            if (!reader.ReadNull())
                return null;
            return reader.ReadString();
        }

        success = false;
        return 0;
    }

    public void Write(BinaryWriter writer, Type type, object value, out bool success)
    {
        success = true;

        if (type == typeof(sbyte)) { writer.Write((sbyte)value); return; }
        if (type == typeof(byte)) { writer.Write((byte)value); return; }
        if (type == typeof(bool)) { writer.Write((bool)value); return; }

        if (type == typeof(short)) { writer.Write((short)value); return; }
        if (type == typeof(ushort)) { writer.Write((ushort)value); return; }

        if (type == typeof(int)) { writer.Write((int)value); return; }
        if (type == typeof(uint)) { writer.Write((uint)value); return; }

        if (type == typeof(long)) { writer.Write((long)value); return; }
        if (type == typeof(ulong)) { writer.Write((ulong)value); return; }

        if (type == typeof(float)) { writer.Write((float)value); return; }
        if (type == typeof(double)) { writer.Write((double)value); return; }
        if (type == typeof(decimal)) { writer.Write((decimal)value); return; }

        if (type == typeof(char)) { writer.Write((char)value); return; }
        if (type == typeof(string)) 
        {
            if (!writer.WriteNull(value))
                return; 
            writer.Write((string)value); 
            return; 
        }

        success = false;
    }
}