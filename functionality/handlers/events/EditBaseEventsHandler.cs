using System.Collections.Generic;
using EditorCoop.Functionality.Data.Events;
using EditorCoop.Functionality.Packets.Events;
using EditorCoop.Patches;
using RDLevelEditor;

namespace EditorCoop.Functionality.Handlers.Events;

public class EditBaseEventsHandler : Handler
{
    public static void Run(EditBaseEventsPacket packet)
    {
        foreach (BaseEventEditData editData in packet.Edits)
        {
            if (!UIDHolder.UIDToEventControl.TryGetValue(editData.UID, out LevelEventControl_Base control))
                continue;

            LevelEvent_Base levelEvent = control.levelEvent;
            
            if (editData.Conditionals is List<int> conditionals)
                levelEvent.conditionals = conditionals;

            if (editData.GlobalConditionals is List<string> globalConditionals)
                levelEvent.globalConditionals = globalConditionals;

            if (editData.Rooms is int[] rooms)
                levelEvent.rooms = rooms;

            if (editData.Tag is string tag)
                levelEvent.tag = tag;

            levelEvent.tagRunNormally = editData.RunTagNormally;

            control.UpdateUI();
            Editor.UpdateTimelineAccordingToLevelEventType(levelEvent.type);

            InspectorPanelUpdateUI();
            FlagUnsavedChanges();
        }
    }
}