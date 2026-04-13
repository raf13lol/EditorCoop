using System.Collections.Generic;
using EditorCoop.Functionality.Data.Events;
using EditorCoop.Functionality.Packets.Events;
using EditorCoop.Patches;
using RDLevelEditor;

namespace EditorCoop.Functionality.Handlers.Events;

public class EditEventsHandler : Handler
{
    public static void Run(EditEventsPacket packet)
    {
        foreach (EventEditData editData in packet.Edits)
        {
            if (!editData.FoundUID)
                return;

            LevelEvent_Base levelEvent = editData.LevelEvent;
            foreach (PropertyEditData propertyEditData in editData.PropertiesChanged)
            {
                BasePropertyInfo propertyInfo = editData.PropertiesInfo[propertyEditData.Index];
                propertyInfo.propertyInfo.SetValue(levelEvent, propertyEditData.Value);
            }

            if (editData.BaseProperties.Conditionals is List<int> conditionals)
                levelEvent.conditionals = conditionals;

            if (editData.BaseProperties.GlobalConditionals is List<string> globalConditionals)
                levelEvent.globalConditionals = globalConditionals;

            if (editData.BaseProperties.Rooms is int[] rooms)
                levelEvent.rooms = rooms;

            if (editData.BaseProperties.Tag is string tag)
                levelEvent.tag = tag;

            levelEvent.tagRunNormally = editData.BaseProperties.RunTagNormally;

            editData.LevelEventControl.UpdateUI();
            Editor.UpdateTimelineAccordingToLevelEventType(levelEvent.type);

            InspectorPanelUpdateUI();
            FlagUnsavedChanges();
        }
    }
}