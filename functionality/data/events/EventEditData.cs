using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using Network.Packets;
using RDLevelEditor;

namespace EditorCoop.Functionality.Data.Events;

public class EventEditData() : IPacketData
{
    public bool FoundUID;

    public LevelEventControl_Base LevelEventControl;
    public LevelEvent_Base LevelEvent;
    public ImmutableList<BasePropertyInfo> PropertiesInfo;

    public int UID;

    public List<PropertyEditData> PropertiesChanged;

    public void Decode(BinaryReader reader)
    {
        UID = reader.ReadInt32();
        FoundUID = UIDHolder.UIDToEventControl.TryGetValue(UID, out LevelEventControl);

        int offset = reader.ReadInt32();
        if (!FoundUID)
        {
            reader.BaseStream.Position += offset;
            return;
        }

        int count = reader.ReadInt32();
        PropertiesChanged = new(count);

        LevelEvent = LevelEventControl.levelEvent;
        PropertiesInfo = LevelEvent.info.propertiesInfo;

        for (int i = 0; i < count; i++)
        {
            int index = reader.ReadInt32();

            BasePropertyInfo propertyInfo = PropertiesInfo[index];
            object value = reader.Read(propertyInfo.propertyInfo.PropertyType);

            PropertiesChanged.Add(new()
            {
                Index = index,
                Value = value
            });
        }
    }

    public void Encode(BinaryWriter writer)
    {
        writer.Write(UID);

        writer.Write(0); // temp value for offset
        long offsetPosition = writer.BaseStream.Position;

        writer.Write(PropertiesChanged.Count);
        
        PropertiesInfo = UIDHolder.UIDToEventControl[UID].levelEvent.info.propertiesInfo;
        foreach (PropertyEditData editData in PropertiesChanged)
        {
            writer.Write(editData.Index);
            writer.Write(PropertiesInfo[editData.Index].propertyInfo.PropertyType, editData.Value);
        }

        long endPosition = writer.BaseStream.Position;
        int offset = (int)(endPosition - offsetPosition);

        writer.BaseStream.Position = offsetPosition - sizeof(int);
        writer.Write(offset);
        writer.BaseStream.Position = endPosition;
    }
}