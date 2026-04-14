using System;
using EditorCoop.Functionality.Data.Events;
using EditorCoop.Functionality.Packets.Events;
using RDLevelEditor;

namespace EditorCoop.Functionality.Handlers.Events;

public class CreateEventsHandler : Handler
{
    public static void Run(CreateEventsPacket packet)
    {
        foreach (EventCreationData newEventData in packet.Events)
        {
            Type newEventType = Type.GetType($"RDLevelEditor.LevelEvent_{newEventData.EventType}, Assembly-CSharp");
            LevelEvent_Base newEvent = (LevelEvent_Base)Activator.CreateInstance(newEventType);

            newEvent.uid = newEventData.UID;

            newEvent.bar = newEventData.Bar;
            newEvent.beat = newEventData.Beat;

            newEvent.tab = newEventData.Tab;
            newEvent.y = newEventData.Y;

            newEvent.row = newEventData.Row;
            newEvent.target = newEventData.Target;

            if (newEventData.CopyEventUID is int copyEventUID
            && UIDHolder.UIDToEventControl.TryGetValue(copyEventUID, out LevelEventControl_Base copyControl))
            {
                newEvent.CopyFromInternal(copyControl.levelEvent);
                if (newEvent is LevelEvent_AddOneshotBeat oneshot)
                    oneshot.CapXPosForPositiveFreezeBurnCueTime();
            }

            if (newEventData.Tab == Tab.Sprites)
                newEvent.row = SpriteHeader.GetSpriteDataIndex(newEventData.Target);

            newEvent.OnCreate();

            LevelEventControl_Base control = Editor.CreateEventControl(newEvent, newEventData.Tab, true);
            control.UpdateUI();
            Editor.UpdateTimelineAccordingToLevelEventType(newEventData.EventType);

            FlagUnsavedChanges();

            UIDHolder.StoreEventControl(control);
        }
    }
}