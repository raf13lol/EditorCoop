using System;
using System.IO;
using HarmonyLib;
using Network.Packets;
using RDLevelEditor;
using UnityEngine;

namespace EditorCoop.Functionality;

public class RDPacketProvider : IPacketProvider
{
    private static readonly Func<object, object> FloatExpressionGetExp = AccessTools.Field(typeof(FloatExpression), "exp").GetValue;
    private static readonly Func<object, object> FloatExpressionGetNum = AccessTools.Field(typeof(FloatExpression), "num").GetValue;
    private static readonly Action<object, object> FloatExpressionSetExp = AccessTools.Field(typeof(FloatExpression), "exp").SetValue;

    public object Read(BinaryReader reader, Type type, out bool success)
    {
        success = true;

        if (type == typeof(ColorOrPalette))
        {
            bool isPalette = reader.ReadBoolean();
            if (isPalette)
            {
                int index = reader.ReadInt32();
                return ColorOrPalette.FromString("pal" + index);
            }

            float r = reader.ReadSingle();
            float g = reader.ReadSingle();
            float b = reader.ReadSingle();
            float a = reader.ReadSingle();
            
            Color colour = new(r, g, b, a);
            return (ColorOrPalette)colour;
        }

        if (type == typeof(SoundDataStruct))
        {
            string filename = reader.ReadString();
            int volume = reader.ReadInt32();
            int pitch = reader.ReadInt32();
            int pan = reader.ReadInt32();
            int offset = reader.ReadInt32();
            bool used = reader.ReadBoolean();

            return new SoundDataStruct(filename, volume, pitch, pan, offset, used);
        }

        if (type == typeof(BarAndBeat))
        {
            int bar = reader.ReadInt32();
            float beat = reader.ReadSingle();
            return new BarAndBeat(bar, beat);
        }

        if (type == typeof(Vector2))
        {
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            return new Vector2(x, y);
        }

        if (type == typeof(Float2))
        {
            float? x = PacketBinary.Read<float?>(reader);
            float? y = PacketBinary.Read<float?>(reader);

            Float2 f2 = Float2.empty;

            if (f2.xUsed = x != null)
                f2.x = (float)x;
            if (f2.yUsed = y != null)
                f2.y = (float)y;

            return f2;
        }

        if (type == typeof(FloatExpression))
        {
            FloatExpression fexp;

            bool isExpression = reader.ReadBoolean();
            if (isExpression)
            {
                fexp = FloatExpression.EmptyInput();
                FloatExpressionSetExp(fexp, reader.ReadString());
            }
            else
                fexp = new(reader.ReadSingle());
            return fexp;
        }

        if (type == typeof(FloatExpression2))
        {
            FloatExpression x = (FloatExpression)Read(reader, typeof(FloatExpression), out bool _);
            FloatExpression y = (FloatExpression)Read(reader, typeof(FloatExpression), out bool _);
            return new FloatExpression2(x, y);
        }

        success = false;
        return 0;
    }

    public void Write(BinaryWriter writer, Type type, object value, out bool success)
    {
        success = true;

        if (type == typeof(ColorOrPalette))
        {
            ColorOrPalette col = (ColorOrPalette)value;
            bool isPalette = col.isPalette;
            writer.Write(isPalette);

            if (isPalette)
            {
                writer.Write(col.GetIndex());
                return;
            }

            Color colour = col.ToColor();

            writer.Write(colour.r);
            writer.Write(colour.g);
            writer.Write(colour.b);
            writer.Write(colour.a);
            return;
        }

        if (type == typeof(SoundDataStruct))
        {
            SoundDataStruct sound = (SoundDataStruct)value;

            writer.Write(sound.filename);
            writer.Write(sound.volume);
            writer.Write(sound.pitch);
            writer.Write(sound.pan);
            writer.Write(sound.offset);
            writer.Write(sound.used);
            return;
        }

        if (type == typeof(BarAndBeat))
        {
            BarAndBeat barAndBeat = (BarAndBeat)value;

            writer.Write(barAndBeat.bar);
            writer.Write(barAndBeat.beat);
            return;
        }

        if (type == typeof(Vector2))
        {
            Vector2 vec = (Vector2)value;

            writer.Write(vec.x);
            writer.Write(vec.y);
            return;
        }

        if (type == typeof(Float2))
        {
            Float2 f2 = (Float2)value;
            float? x = f2.xUsed ? f2.x : null;
            float? y = f2.yUsed ? f2.y : null;

            PacketBinary.Write(writer, x);
            PacketBinary.Write(writer, y);
            return;
        }

        if (type == typeof(FloatExpression))
        {
            FloatExpression fexp = (FloatExpression)value;

            writer.Write(fexp.isExpression);
            if (fexp.isExpression)
                writer.Write((string)FloatExpressionGetExp(fexp));
            else
                writer.Write((float)FloatExpressionGetNum(fexp));
            return;
        }

        if (type == typeof(FloatExpression2))
        {
            FloatExpression2 fexp2 = (FloatExpression2)value;

            Write(writer, typeof(FloatExpression), fexp2.x, out bool _); 
            Write(writer, typeof(FloatExpression), fexp2.y, out bool _); 
            return;
        }

        success = false;
    }
}