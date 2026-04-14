using System;
using System.IO;

namespace Network.Packets;

public class LiteralPacketProvider : IPacketProvider
{
    public bool Read(BinaryReader reader, Type type, out object value)
    {
        if (type == typeof(sbyte)) { value = reader.ReadSByte(); return true; }
        if (type == typeof(byte)) { value = reader.ReadByte(); return true; }
        if (type == typeof(bool)) { value = reader.ReadBoolean(); return true; }

        if (type == typeof(short)) { value = reader.ReadInt16(); return true; }
        if (type == typeof(ushort)) { value = reader.ReadUInt16(); return true; }

        if (type == typeof(int)) { value = reader.ReadInt32(); return true; }
        if (type == typeof(uint)) { value = reader.ReadUInt32(); return true; }

        if (type == typeof(long)) { value = reader.ReadUInt64(); return true; }
        if (type == typeof(ulong)) { value = reader.ReadUInt64(); return true; }

        if (type == typeof(float)) { value = reader.ReadSingle(); return true; }
        if (type == typeof(double)) { value = reader.ReadDouble(); return true; }
        if (type == typeof(decimal)) { value = reader.ReadDecimal(); return true; }

        if (type == typeof(char)) { value = reader.ReadChar(); return true; }
        if (type == typeof(string))
        {
            if (!reader.ReadNull())
                value = null;
            else 
                value = reader.ReadString();
            return true;
        }

        value = null;
        return false;
    }

    public bool Write(BinaryWriter writer, Type type, object value)
    {
        if (type == typeof(sbyte)) { writer.Write((sbyte)value); return true; }
        if (type == typeof(byte)) { writer.Write((byte)value); return true; }
        if (type == typeof(bool)) { writer.Write((bool)value); return true; }

        if (type == typeof(short)) { writer.Write((short)value); return true; }
        if (type == typeof(ushort)) { writer.Write((ushort)value); return true; }

        if (type == typeof(int)) { writer.Write((int)value); return true; }
        if (type == typeof(uint)) { writer.Write((uint)value); return true; }

        if (type == typeof(long)) { writer.Write((long)value); return true; }
        if (type == typeof(ulong)) { writer.Write((ulong)value); return true; }

        if (type == typeof(float)) { writer.Write((float)value); return true; }
        if (type == typeof(double)) { writer.Write((double)value); return true; }
        if (type == typeof(decimal)) { writer.Write((decimal)value); return true; }

        if (type == typeof(char)) { writer.Write((char)value); return true; }
        if (type == typeof(string)) 
        {
            if (writer.WriteNull(value))
                writer.Write((string)value); 
            return true; 
        }

        return false;
    }
}