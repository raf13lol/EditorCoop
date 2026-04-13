using EditorCoop.Functionality.Packets.Events;
using RDLevelEditor;

namespace EditorCoop.Functionality.Handlers.Events;

public class DeleteEventsHandler : Handler
{
    public static void Run(DeleteEventsPacket packet)
    {
        foreach (int controlUID in packet.UIDs)
        {
            if (!UIDHolder.UIDToEventControl.TryGetValue(controlUID, out LevelEventControl_Base control))
                continue;

            // Copied from scnEditor.DeleteEventControl due to forced save state
            Editor.ClearHoveredEventControl();

            control.isShownAsSelected = false;
            control.levelEvent.OnDelete();

            try
            {
                control.container.Remove(control);
            }
            catch
            { }

            Editor.eventControls.Remove(control);
            Editor.selectedControls.Remove(control);
            Editor.UpdateTimelineAccordingToLevelEventType(control.levelEvent.type);

            if (control != null && control.gameObject != null)
                UnityEngine.Object.Destroy(control.gameObject);

            InspectorPanelUpdateUI();
            FlagUnsavedChanges();
        }
    }
}