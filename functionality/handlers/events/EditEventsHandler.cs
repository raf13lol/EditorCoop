using EditorCoop.Functionality.Data.Events;
using EditorCoop.Functionality.Packets.Events;
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

            editData.LevelEventControl.UpdateUI();
            Editor.UpdateTimelineAccordingToLevelEventType(levelEvent.type);

            InspectorPanelUpdateUI();
            FlagUnsavedChanges();
        }
    }
}