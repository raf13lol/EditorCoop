using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using EditorCoop.Patches;

namespace Network.Packets;

public static class PacketBinary
{
    private static readonly Dictionary<Type, ConstructorInfo> ListConstructors = [];

    public static readonly List<IPacketProvider> Providers = [
        new LiteralPacketProvider()
    ];

    public static bool ReadNull(this BinaryReader reader)
    {
        return reader.ReadBoolean();
    }

    public static object Read(this BinaryReader reader, Type type)
    {
        if (type.IsGenericType)
        {
            Type genericType = type.GetGenericTypeDefinition();
            Type[] argumentTypes = type.GetGenericArguments();

            if (genericType == typeof(Nullable<>))
            {
                if (!reader.ReadNull())
                    return null;
                return Read(reader, argumentTypes[0]);
            }

            if (genericType == typeof(List<>))
            {
                if (!reader.ReadNull())
                    return null;

                Type elementType = argumentTypes[0];
                if (!ListConstructors.TryGetValue(elementType, out ConstructorInfo? listConstructor))
                    ListConstructors[elementType] = listConstructor = type.GetConstructor([typeof(int)]);

                int count = reader.ReadInt32();
                IList list = (IList)Activator.CreateInstance(type);
                listConstructor.Invoke(list, [count]);

                for (int i = 0; i < count; i++)
                    list.Add(Read(reader, elementType));
                return list;
            }

            if (genericType == typeof(Dictionary<,>))
            {
                if (!reader.ReadNull())
                    return null;

                Type keyType = argumentTypes[0];
                Type valueType = argumentTypes[1];

                int count = reader.ReadInt32();
                IDictionary dictionary = (IDictionary)type.GetConstructor([]).Invoke(null, []);

                for (int i = 0; i < count; i++)
                    dictionary[Read(reader, keyType)] = Read(reader, valueType);
                return dictionary;
            }
        }

        if (type.BaseType == typeof(Array))
        {
            if (!reader.ReadNull())
                return null;

            Type elementType = type.GetElementType();

            int length = reader.ReadInt32();
            Array array = Array.CreateInstance(elementType, length);

            for (int i = 0; i < length; i++)
                array.SetValue(Read(reader, elementType), i);
            return array;
        }

        if (type.GetInterface(nameof(IPacketData)) != null)
        {
            IPacketData data = (IPacketData)Activator.CreateInstance(type);
            data.Decode(reader);
            return data;
        }

        if (type.IsEnum)
            return Read(reader, type.GetEnumUnderlyingType());

        foreach (IPacketProvider provider in Providers)
        {
            object value = provider.Read(reader, type, out bool success);
            if (success)
                return value;
        }
        return 0;
    }

    public static bool WriteNull(this BinaryWriter writer, object value)
    {
        bool hasValue = value != null;
        writer.Write(hasValue);
        return hasValue;
    }

    public static void Write(this BinaryWriter writer, Type type, object value)
    {
        if (type.IsGenericType)
        {
            Type genericType = type.GetGenericTypeDefinition();
            Type[] argumentTypes = type.GetGenericArguments();

            if (genericType == typeof(Nullable<>))
            {
                if (!writer.WriteNull(value))
                    return;

                Write(writer, argumentTypes[0], value);
                return;
            }

            if (genericType == typeof(List<>))
            {
                if (!writer.WriteNull(value))
                    return;

                Type elementType = argumentTypes[0];

                IList list = (IList)value;
                int count = list.Count;

                writer.Write(count);
                for (int i = 0; i < count; i++)
                    Write(writer, elementType, list[i]);

                return;
            }

            if (genericType == typeof(Dictionary<,>))
            {
                if (!writer.WriteNull(value))
                    return;

                Type keyType = argumentTypes[0];
                Type valueType = argumentTypes[1];

                IDictionary dictionary = (IDictionary)value;
                int count = dictionary.Count;

                writer.Write(count);
                foreach (KeyValuePair<object, object?> kvp in dictionary)
                {
                    Write(writer, keyType, kvp.Key);
                    Write(writer, valueType, kvp.Value);
                }

                return;
            }
        }

        if (type.BaseType == typeof(Array))
        {
            if (!writer.WriteNull(value))
                return;

            Type elementType = type.GetElementType();

            Array array = (Array)value;
            int length = array.Length;

            writer.Write(length);
            for (int i = 0; i < length; i++)
                Write(writer, elementType, array.GetValue(i));

            return;
        }

        if (type.GetInterface(nameof(IPacketData)) != null)
        {
            ((IPacketData)value).Encode(writer);
            return;
        }

        if (type.IsEnum)
        {
            Type underlyingType = type.GetEnumUnderlyingType();
            Write(writer, underlyingType, Convert.ChangeType(value, underlyingType));
            return;
        }

        foreach (IPacketProvider provider in Providers)
        {
            provider.Write(writer, type, value, out bool success);
            if (success)
                return;
        }
    }

    public static T Read<T>(this BinaryReader reader)
        => (T)Read(reader, typeof(T));

    public static void Write<T>(this BinaryWriter writer, T value)
        => Write(writer, typeof(T), value);
}