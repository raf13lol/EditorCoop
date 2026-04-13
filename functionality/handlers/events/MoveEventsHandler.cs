using EditorCoop.Compatibility;
using EditorCoop.Functionality.Data.Events;
using EditorCoop.Functionality.Packets.Events;
using RDLevelEditor;

namespace EditorCoop.Functionality.Handlers.Events;

public class MoveEventsHandler : Handler
{
    public static void Run(MoveEventsPacket packet)
    {
        foreach (EventMovementData move in packet.Movements)
        {
            if (!UIDHolder.UIDToEventControl.TryGetValue(move.UID, out LevelEventControl_Base control))
                continue;

            LevelEvent_Base levelEvent = control.levelEvent;

            levelEvent.bar = move.Bar;
            levelEvent.beat = move.Beat;

            levelEvent.y = move.Y;

            if (PluginCompatibility.RDEditorPlusDetected)
                RDEditorPlusCompatibility.SetEventSubRowY(levelEvent, move.SubRowY);

            levelEvent.row = move.Row;
            levelEvent.target = move.Target;

            if (levelEvent.tab == Tab.Sprites)
                levelEvent.row = SpriteHeader.GetSpriteDataIndex(move.Target);

            control.UpdateUI();
            Editor.UpdateTimelineAccordingToLevelEventType(levelEvent.type);

            InspectorPanelUpdateUI();
            FlagUnsavedChanges();
        }
    }
}